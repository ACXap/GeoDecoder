// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding.GeoCodingService
{
    public class RusGisDemoGeoCodingService : GeoService, IGeoCodingService
    {
        #region PrivateConst
        /// <summary>
        /// Ссылка на геокодер яндекса
        /// </summary>
        protected override string _url => @"https://capex.cloud.rt.ru/view/rest/gp/geocoding?geocode=";
        
        /// <summary>
        /// Ошибка при превышении лимита в сутки
        /// </summary>
        protected override string _errorWebRequestLimit => "Удаленный сервер возвратил ошибку: (500) Внутренняя ошибка сервера.";
        /// <summary>
        /// Ошибка если привешено время ожидания (скорее всего сайт упал)
        /// </summary>
        protected override string _errorWebRequestTimeIsUp => "Удаленный сервер возвратил ошибку: (504) Истекло время ожидания шлюза.";
        #endregion PrivateConst

        /// <summary>
        /// Название геосервиса
        /// </summary>
        public override string Name => "RusGisDemo";

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
                List<RusGisJson> list = JsonConvert.DeserializeObject<List<RusGisJson>>(json);
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

        private GeoCod GetGeo(RusGisJson geo)
        {
            return new GeoCod()
            {
                Text = geo.Text,
                Kind = geo.Kind,
                Precision = geo.Precision,
                Latitude = geo.PosX.ToString(),
                Longitude = geo.PosY.ToString()
            };
        }
    }
}