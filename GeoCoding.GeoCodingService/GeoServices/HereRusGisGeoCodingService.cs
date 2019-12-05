// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding.GeoCodingService
{
    public class HereRusGisGeoCodingService : GeoService, IGeoCodingService
    {
        #region PrivateField
        /// <summary>
        /// Ссылка на геокодер хереРусГис
        /// </summary>
        protected override string _url => @"https://master.rcloud.cloud.rt.ru/api/geocoding?request=";

        /// <summary>
        /// Ошибка при превышении лимита в сутки
        /// </summary>
        protected override string _errorWebRequestLimit => "Удаленный сервер возвратил ошибку: (500) Внутренняя ошибка сервера.";
        /// <summary>
        /// Ошибка если привешено время ожидания (скорее всего сайт упал)
        /// </summary>
        protected override string _errorWebRequestTimeIsUp => "Удаленный сервер возвратил ошибку: (504) Истекло время ожидания шлюза.";
        #endregion PrivateField

        /// <summary>
        /// Название геосервиса
        /// </summary>
        public override string Name => "HereRusGis";

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        public override string GetUrlRequest(string address, List<double> polygon)
        {
            return $"{_url}{address}&geoCoderType=HERE";
        }

        /// <summary>
        /// Метод преобразования json в объекты
        /// </summary>
        /// <param name="callback">Функция обратного вызова, с параметром объект, ошибка</param>
        /// <param name="json">Строка json</param>
        protected override void ParserJson(Action<IEnumerable<GeoCod>, Exception> callback, string json)
        {
            Exception error = null;
            IEnumerable<GeoCod> data = null;

            try
            {
                List<HereJson> list = JsonConvert.DeserializeObject<List<HereJson>>(json);
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

        private GeoCod GetGeo(HereJson geo)
        {
            return new GeoCod()
            {
                Text = geo.Text,
                Kind = geo.Kind,
                Precision = geo.Precision,
                Latitude = geo.PosY.ToString(),
                Longitude = geo.PosX.ToString(),
            };
        }
    }
}