using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    public class SputnikGeoCodingService : IGeoCodingService
    {
        private const string _url = @"http://search.maps.sputnik.ru/search?q=";

        public string Name => "Sputnik";

        /// <summary>
        /// Метод для получения геоокординат по адресу
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: объект, ошибка</param>
        /// <param name="address">Строка адреса для поиска</param>
        public void GetGeoCod(Action<GeoCod, Exception> callback, string address)
        {
            Exception error = null;
            GeoCod geocod = null;

            if (!string.IsNullOrEmpty(address))
            {
                GetJsonString((s, e) =>
                {
                    error = e;
                    if (e == null && !string.IsNullOrEmpty(s))
                    {
                        ParserJson((g, er) =>
                        {
                            error = er;
                            if (error == null)
                            {
                                geocod = g;
                            }
                        }, s);
                    }
                }, address);
            }
            else
            {
                error = new ArgumentNullException();
            }

            callback(geocod, error);
        }

        /// <summary>
        /// Метод для получения json ответа от sputnik
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметроми строка, ошибка</param>
        /// <param name="address">Строка адреса для поиска координат</param>
        private void GetJsonString(Action<string, Exception> callback, string address)
        {
            Exception error = null;
            string json = string.Empty;
            string url = GetUrlRequest(address);

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Headers.Add("Content-Encoding: gzip, deflate, br");
                request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        if (dataStream != null)
                        {
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                json = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(json, error);
        }

        private void ParserJson(Action<GeoCod, Exception> callback, string json)
        {
            Exception error = null;
            GeoCod geocod = null;

            try
            {
                SputnikJsonOldFormat a = JsonConvert.DeserializeObject<SputnikJsonOldFormat>(json);
                byte countFound = (byte)a.Result.Count;

                if (a.Result.Count == 1)
                {
                    geocod = GetGeo(a.Result[0], 1);
                }
                else
                {
                    var o = a.Result.Where(x => x.FullMatch && x.Type == "house");
                    if (o.Count() == 1)
                    {
                        geocod = GetGeo(o.FirstOrDefault(), 1);
                    }
                    else
                    {
                        geocod = GetGeo(null, a.Result.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(geocod, error);
        }

        private GeoCod GetGeo(Result g, int coutResult)
        {
            if(g!=null)
            {
                return new GeoCod()
                {
                    Text = g.DisplayName,
                    Kind = g.Type,
                    Precision = g.FullMatch.ToString(),
                    Latitude = g.Position.Lat.ToString(),
                    Longitude = g.Position.Lon.ToString(),
                    CountResult = (byte)coutResult
                };
            }
            return new GeoCod() { CountResult = (byte)coutResult };
            
        }

        public string GetUrlRequest(string address)
        {
            return $"{_url}{address}";
        }
    }
}