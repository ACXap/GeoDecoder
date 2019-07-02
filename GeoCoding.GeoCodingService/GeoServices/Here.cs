using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GeoCoding.GeoCodingService.Helpers;
using Newtonsoft.Json;
using GeoCoding.GeoCodingService.Data;

namespace GeoCoding.GeoCodingService
{
    public class HereGeoCodingService : GeoService, IGeoCodingService
    {
        private string _key;
        private string _keyFile = "keyHere";

        /// <summary>
        /// Название геосервиса
        /// </summary>
        public override string Name => "Here";

        /// <summary>
        /// Ссылка на геокодер яндекса
        /// </summary>
        protected override string _url => @"https://geocoder.api.here.com/6.2/geocode.json?";

        protected override void ParserJson(Action<IEnumerable<GeoCod>, Exception> callback, string json)
        {
            Exception error = null;
            IEnumerable<GeoCod> data = null;

            try
            {
                Here h = JsonConvert.DeserializeObject<Here>(json);
                var list = h.Response.View[0].Result;

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

        private GeoCod GetGeo(Data.Result geo)
        {
            return new GeoCod()
            {
                Kind = geo.MatchLevel,
                Latitude = geo.Location.DisplayPosition.Latitude.ToString(),
                Longitude = geo.Location.DisplayPosition.Longitude.ToString(),
                Text = geo.Location.Address.Label,
                Precision = geo.Relevance.ToString()
            };
        }

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        public override string GetUrlRequest(string address)
        {
            var str = _key.Split(' ');
            return $"{_url}app_id={str[0]}&app_code={str[1]}&searchtext={address}";
        }

        public HereGeoCodingService()
        {
            if (File.Exists(_keyFile))
            {
                var str = File.ReadAllText(_keyFile);
                ProtectedDataDPAPI.DecryptData((s, e) =>
                {
                    _key = s;
                }, str);
            }
        }
    }
}