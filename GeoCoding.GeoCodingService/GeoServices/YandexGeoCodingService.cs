using Newtonsoft.Json;
using System;
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
        public string Name => "YANDEX";

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
                YandexJson ya = JsonConvert.DeserializeObject<YandexJson>(json);
                var list = ya.Response.GeoObjectCollection.FeatureMember;

                if(list.Count == 1)
                {
                    geocod = GetGeo(list[0], 1);
                }
                else
                {
                    var l = list.Where(x => x.GeoObject.MetaDataProperty.GeocoderMetaData.Precision == "exact");
                    if(l.Count() ==1)
                    {
                        geocod = GetGeo(l.FirstOrDefault(), 1);
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

        private GeoCod GetGeo(FeatureMember geo, int coutResult)
        {
            if (geo != null)
            {
                var g = geo.GeoObject.MetaDataProperty.GeocoderMetaData;
                var p = geo.GeoObject.Point.Pos.Split(' ');
                return new GeoCod()
                {
                    Text = g.Text,
                    Kind = g.Kind,
                    Precision = g.Precision,
                    Latitude = p[1],
                    Longitude = p[0],
                    CountResult = coutResult
                };
            }
            return new GeoCod() { CountResult = coutResult };
        }
    }
}