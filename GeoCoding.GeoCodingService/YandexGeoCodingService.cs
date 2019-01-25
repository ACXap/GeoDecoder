using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace GeoCoding.GeoCodingService
{
    public class YandexGeoCodingService : IGeoCodingService
    {
        private const string yandexUrl = @"https://geocode-maps.yandex.ru/1.x/";

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

        private void GetJsonString(Action<string, Exception> callback, string address)
        {
            Exception error = null;
            string json = string.Empty;
            string url = $"{yandexUrl}?geocode={address}&format=json";

            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Headers.Add("Content-Encoding: gzip, deflate, br");
                request.UseDefaultCredentials = true;
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
                        geocod = new GeoCod() { CountResult = count };
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