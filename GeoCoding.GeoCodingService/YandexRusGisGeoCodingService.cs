using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    public class YandexRusGisGeoCodingService : IGeoCodingService
    {
        private const string _url = @"https://master.rcloud.cloud.rt.ru/api/geocoding?request=";
        private const string _errorWebRequest = "Удаленный сервер возвратил ошибку: (500) Внутренняя ошибка сервера.";
        private const string _textError = "Ваш лимит исчерпан";

        public string Name => "YandexRusGis";

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
                error = new ArgumentNullException("Значение адреса пусто");
            }

            callback(geocod, error);
        }

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
                //if (wex.Response != null)
                //{
                //    using (Stream dataStream = wex.Response.GetResponseStream())
                //    {
                //        if (dataStream != null)
                //        {
                //            using (StreamReader reader = new StreamReader(dataStream))
                //            {
                //                var a = reader.ReadToEnd();
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    error = wex;
                //}

                if (wex.Message == _errorWebRequest)
                {
                    error = new Exception(_textError, wex);
                }
                else
                {
                    error = wex;
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
                List<YandexRusGisJson> list = JsonConvert.DeserializeObject<List<YandexRusGisJson>>(json);
                if (list.Count == 1)
                {
                    geocod = GetGeo(list.FirstOrDefault());
                    geocod.CountResult = 1;
                }
                else
                {
                    var a = list.Where(x => x.Precision == "exact");
                    if (a.Any() && a.Count() == 1)
                    {
                        geocod = GetGeo(a.FirstOrDefault());
                        geocod.CountResult = 1;
                    }
                    else
                    {
                        geocod = new GeoCod() { CountResult = (byte)list.Count };
                    }
                }

            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(geocod, error);
        }

        private GeoCod GetGeo(YandexRusGisJson geo)
        {
            return new GeoCod()
            {
                Text = geo.Text,
                Kind = geo.Kind,
                Precision = geo.Precision,
                Latitude = geo.PosY.ToString().Replace(',', '.'),
                Longitude = geo.PosX.ToString().Replace(',', '.')
            };
        }

        public string GetUrlRequest(string address)
        {
            return $"{_url}{address}&geoCoderType=YANDEX";
        }
    }
}