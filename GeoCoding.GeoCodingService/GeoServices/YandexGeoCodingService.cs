// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Newtonsoft.Json;
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

      //  public override bool CanUsePolygon => true;

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        public override string GetUrlRequest(string address, List<double> polygon)
        {
            var box = string.Empty;

            if (polygon!=null && polygon.Count==4)
            {
                box = $"&bbox={DoubleToString(polygon[0])},{DoubleToString(polygon[1])}~{DoubleToString(polygon[2])},{DoubleToString(polygon[3])}&rspn=1";
            }

            return $"{_url}{address}&format=json{box}";
        }

        /// <summary>
        /// Метод преобразования json в объекты
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметроми объект, ошибка</param>
        /// <param name="json">Строка json</param>
        protected override void ParserJson(Action<IEnumerable<GeoCod>, Exception> callback, string json)
        {
            Exception error = null;
            IEnumerable<GeoCod> data = null;

            try
            {
                YandexJson ya = JsonConvert.DeserializeObject<YandexJson>(json);
                var list = ya.Response.GeoObjectCollection.FeatureMember;

                data = list.Select(x =>
                {
                    return GetGeo(x);
                }).ToList();
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(data, error);
        }

        private GeoCod GetGeo(FeatureMember geo)
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
            };
        }
    }
}