using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    /// <summary>
    /// Реализация интерфейса IGeoCodingService для Yandex
    /// </summary>
    public class YandexGeoCodingService : IGeoCodingService
    {
        #region PrivateConst
        /// <summary>
        /// Ссылка на геокодер яндекса
        /// </summary>
        private const string yandexUrl = @"https://geocode-maps.yandex.ru/1.x/";
        /// <summary>
        /// Ошибка при привышении лимита в сутки
        /// </summary>
        private const string _errorWebRequest429 = "Удаленный сервер возвратил ошибку: (429) Unknown status.";
        /// <summary>
        /// Сообщение по поводу превышения лимита в сутки
        /// </summary>
        private const string _textForError429 = "Ваш лимит исчерпан";
        #endregion PrivateConst

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
            string url = $"{yandexUrl}?geocode={address}&format=json";

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
                if(wex.Message == _errorWebRequest429)
                {
                    error = new Exception(_textForError429, wex);
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

        /// <summary>
        /// Метод преобразования json в объекты
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметроми объект, ошибка</param>
        /// <param name="json">Строка json</param>
        private void ParserJson(Action<GeoCod, Exception> callback, string json)
        {
            Exception error = null;
            GeoCod geocod = null;

            try
            {
                JObject a = JObject.Parse(json);
                var countFound = (string)((JValue)a["response"]["GeoObjectCollection"]["metaDataProperty"]["GeocoderResponseMetaData"]["found"]).Value;
                if (byte.TryParse(countFound, out byte count))
                {
                    if (count == 1)
                    {
                        geocod = new GeoCod
                        {
                            Kind = (string)((JValue)a["response"]["GeoObjectCollection"]["featureMember"].First["GeoObject"]["metaDataProperty"]["GeocoderMetaData"]["kind"]).Value,
                            Precision = (string)((JValue)a["response"]["GeoObjectCollection"]["featureMember"].First["GeoObject"]["metaDataProperty"]["GeocoderMetaData"]["precision"]).Value,
                            Text = (string)((JValue)a["response"]["GeoObjectCollection"]["featureMember"].First["GeoObject"]["metaDataProperty"]["GeocoderMetaData"]["text"]).Value,
                            CountResult = count
                        };
                        var point = ((JValue)a["response"]["GeoObjectCollection"]["featureMember"].First["GeoObject"]["Point"]["pos"]).Value.ToString().Split(' ');
                        geocod.Latitude = point[1];
                        geocod.Longitude = point[0];
                    }
                    else
                    {
                        var list = new List<GeoCod>();

                        var ad = a["response"]["GeoObjectCollection"]["featureMember"].Children();
                        foreach (var item in ad)
                        {
                            var g = new GeoCod()
                            {
                                Kind = (string)((JValue)item["GeoObject"]["metaDataProperty"]["GeocoderMetaData"]["kind"]).Value,
                                Precision = (string)((JValue)item["GeoObject"]["metaDataProperty"]["GeocoderMetaData"]["precision"]).Value,
                                Text = (string)((JValue)item["GeoObject"]["metaDataProperty"]["GeocoderMetaData"]["text"]).Value
                            };
                            var point = ((JValue)item["GeoObject"]["Point"]["pos"]).Value.ToString().Split(' ');
                            g.Latitude = point[1];
                            g.Longitude = point[0];
                            list.Add(g);
                        }
                        var e = list.Where(x => x.Precision == "exact");
                        if (e.Any() && e.Count() == 1)
                        {
                            geocod = e.First();
                            geocod.CountResult = 1;
                        }
                        else
                        {
                            geocod = new GeoCod() { CountResult = count };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(geocod, error);
        }
    }
}