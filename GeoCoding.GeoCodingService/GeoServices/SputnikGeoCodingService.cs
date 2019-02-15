using Newtonsoft.Json;
using System;
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
        protected override void ParserJson(Action<GeoCod, Exception> callback, string json)
        {
            Exception error = null;
            GeoCod geocod = null;

            try
            {
                SputnikJsonOldFormat a = JsonConvert.DeserializeObject<SputnikJsonOldFormat>(json);

                if (a.Result.Count == 1)
                {
                    geocod = GetGeo(a.Result[0], 1);
                }
                else
                {
                    var o = a.Result.Where(x => x.FullMatch && x.Type == "house");
                    if (o.Count() == 1)
                    {
                        geocod = GetGeo(o.FirstOrDefault(), 1);
                    }
                    else
                    {
                        geocod = GetGeo(null, a.Result.Count);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(geocod, error);
        }

        private GeoCod GetGeo(Result g, int coutResult)
        {
            if (g != null)
            {
                return new GeoCod()
                {
                    Text = g.DisplayName,
                    Kind = g.Type,
                    Precision = g.FullMatch.ToString(),
                    Latitude = g.Position.Lat.ToString(),
                    Longitude = g.Position.Lon.ToString(),
                    CountResult = coutResult
                };
            }
            return new GeoCod() { CountResult = coutResult };
        }
    }
}