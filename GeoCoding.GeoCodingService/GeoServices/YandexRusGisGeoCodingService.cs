using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding.GeoCodingService
{
    /// <summary>
    /// Реализация интерфейса IGeoCodingService
    /// </summary>
    public class YandexRusGisGeoCodingService : GeoService, IGeoCodingService
    {
        #region PrivateConst
        /// <summary>
        /// Ссылка на геокодер яндексаРусГис
        /// </summary>
        protected override string _url => @"https://master.rcloud.cloud.rt.ru/api/geocoding?request=";
        
        /// <summary>
        /// Ошибка при привышении лимита в сутки
        /// </summary>
        protected override string _errorWebRequestLimit => "Удаленный сервер возвратил ошибку: (500) Внутренняя ошибка сервера.";
        #endregion PrivateConst

        /// <summary>
        /// Название геосервиса
        /// </summary>
        public string Name => "YandexRusGis";

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        public override string GetUrlRequest(string address)
        {
            return $"{_url}{address}&geoCoderType=YANDEX";
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
                List<RusGisJson> list = JsonConvert.DeserializeObject<List<RusGisJson>>(json);
                if (list.Count == 1)
                {
                    geocod = GetGeo(list.FirstOrDefault(), 1);
                }
                else
                {
                    var a = list.Where(x => x.Precision == "exact");
                    if (a.Count() == 1)
                    {
                        geocod = GetGeo(a.FirstOrDefault(), 1);
                    }
                    else
                    {
                        geocod = GetGeo(null, list.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(geocod, error);
        }

        private GeoCod GetGeo(RusGisJson geo, int countResult)
        {
            if(geo!=null)
            {
                return new GeoCod()
                {
                    Text = geo.Text,
                    Kind = geo.Kind,
                    Precision = geo.Precision,
                    Latitude = geo.PosY.ToString().Replace(',', '.'),
                    Longitude = geo.PosX.ToString().Replace(',', '.'),
                    CountResult = countResult
                };
            }
            else
            {
                return new GeoCod()
                {
                    CountResult = countResult
                };
            }
        }
    }
}