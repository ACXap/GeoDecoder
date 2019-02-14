using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    public class RusGisDemoGeoCodingService : IGeoCodingService
    {
        private const string _url = @"https://capex.cloud.rt.ru/view/rest/gp/geocoding?geocode=";

        public string Name => "RusGeoDemo";

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
        /// Метод для получения json ответа от яндекса
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
            catch (WebException wex)
            {
                error = wex;
            }

            catch (Exception ex)
            {
                error = ex;
            }

            callback(json, error);
        }

        /// <summary>
        /// Метод преобразования json в объекты
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром объект, ошибка</param>
        /// <param name="json">Строка json</param>
        private void ParserJson(Action<GeoCod, Exception> callback, string json)
        {
            Exception error = null;
            GeoCod geocod = null;

            try
            {
                List<RusGisDemoJson> a = JsonConvert.DeserializeObject<List<RusGisDemoJson>>(json);
                if (a.Any())
                {
                    if (a.Count == 1)
                    {
                        var item = a[0];
                        geocod = new GeoCod()
                        {
                            CountResult = 1,
                            Kind = item.kind,
                            Precision = item.precision,
                            Longitude = item.posY.ToString(),
                            Latitude = item.posX.ToString(),
                            Text = item.text
                        };
                    }
                    else
                    {
                        geocod = new GeoCod() { CountResult = a.Count };
                    }
                }
                else
                {
                    geocod = new GeoCod() { CountResult = 0 };
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(geocod, error);
        }

        public string GetUrlRequest(string address)
        {
            return $"{_url}{address}";
        }
    }
}