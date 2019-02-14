using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding.GeoCodingService
{
    /// <summary>
    /// Реализация интерфейса IGeoCodingService
    /// </summary>
    public class YandexGeoCodingService : GeoService, IGeoCodingService
    {
        #region PrivateConst
        /// <summary>
        /// Ссылка на геокодер яндекса
        /// </summary>
        protected override string _url => @"https://geocode-maps.yandex.ru/1.x/?geocode=";
        
        /// <summary>
        /// Ошибка при привышении лимита в сутки
        /// </summary>
        protected override string _errorWebRequestLimit => "Удаленный сервер возвратил ошибку: (429) Unknown status.";
        #endregion PrivateConst

        /// <summary>
        /// Название геосервиса
        /// </summary>
        public override string Name => "YANDEX";

        /// <summary>
        /// Метод для получения геоокординат по адресу
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметрами: объект, ошибка</param>
        /// <param name="address">Строка адреса для поиска</param>
        public override void GetGeoCod(Action<GeoCod, Exception> callback, string address)
        {
            base.GetGeoCod((g, e) =>
            {
                callback(g, e);
            }, address);
        }

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        public override string GetUrlRequest(string address)
        {
            return $"{_url}{address}&format=json";
        }

        /// <summary>
        /// Метод преобразования json в объекты
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметроми объект, ошибка</param>
        /// <param name="json">Строка json</param>
        protected override void ParserJson(Action<GeoCod, Exception> callback, string json)
        {
            Exception error = null;
            GeoCod geocod = null;

            try
            {
                JObject a = JObject.Parse(json);
                var countFound = (string)((JValue)a["response"]["GeoObjectCollection"]["metaDataProperty"]["GeocoderResponseMetaData"]["found"]).Value;
                if (int.TryParse(countFound, out int count))
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