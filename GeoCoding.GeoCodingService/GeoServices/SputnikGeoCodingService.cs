using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding.GeoCodingService
{
    public class SputnikGeoCodingService : GeoService, IGeoCodingService
    {
        #region PrivateConst
        /// <summary>
        /// Ссылка на геокодер яндекса
        /// </summary>
        protected override string _url => @"http://search.maps.sputnik.ru/search?q=";
        #endregion PrivateConst

        /// <summary>
        /// Название геосервиса
        /// </summary>
        public string Name => "Sputnik";

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
                SputnikJsonOldFormat a = JsonConvert.DeserializeObject<SputnikJsonOldFormat>(json);

                data = a.Result.Select(x =>
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

        private GeoCod GetGeo(Result g)
        {
            return new GeoCod()
            {
                Text = g.DisplayName,
                Kind = g.Type,
                Precision = g.FullMatch.ToString(),
                Latitude = g.Position.Lat.ToString(),
                Longitude = g.Position.Lon.ToString(),
            };
        }
    }
}