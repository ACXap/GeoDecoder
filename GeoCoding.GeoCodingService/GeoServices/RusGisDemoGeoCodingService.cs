//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace GeoCoding.GeoCodingService
//{
//    public class RusGisDemoGeoCodingService : GeoService, IGeoCodingService
//    {
//        #region PrivateConst
//        /// <summary>
//        /// Ссылка на геокодер яндекса
//        /// </summary>
//        protected override string _url => @"https://capex.cloud.rt.ru/view/rest/gp/geocoding?geocode=";
//        #endregion PrivateConst
        
//        /// <summary>
//        /// Название геосервиса
//        /// </summary>
//        public string Name => "RusGisDemo";

//        /// <summary>
//        /// Метод преобразования json в объекты
//        /// </summary>
//        /// <param name="callback">Функция обратного вызова, с параметром объект, ошибка</param>
//        /// <param name="json">Строка json</param>
//        protected override void ParserJson(Action<GeoCod, Exception> callback, string json)
//        {
//            Exception error = null;
//            GeoCod geocod = null;

//            try
//            {
//                List<RusGisJson> list = JsonConvert.DeserializeObject<List<RusGisJson>>(json);
//                if (list.Count == 1)
//                {
//                    geocod = GetGeo(list.FirstOrDefault(), 1);
//                }
//                else
//                {
//                    geocod = GetGeo(null, list.Count);
//                }
//            }
//            catch (Exception ex)
//            {
//                error = ex;
//            }

//            callback(geocod, error);
//        }

//        private GeoCod GetGeo(RusGisJson geo, int countResult)
//        {
//            if (geo != null)
//            {
//                return new GeoCod()
//                {
//                    Text = geo.Text,
//                    Kind = geo.Kind,
//                    Precision = geo.Precision,
//                    Latitude = geo.PosY.ToString(),
//                    Longitude = geo.PosX.ToString(),
//                    CountResult = countResult
//                };
//            }
//            else
//            {
//                return new GeoCod()
//                {
//                    CountResult = countResult
//                };
//            }
//        }
//    }
//}