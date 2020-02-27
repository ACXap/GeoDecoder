// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.GeoCodingService.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoCoding.GeoCodingService
{
    public class HereGeoCodingService : GeoService, IGeoCodingService
    {
        private string _key;
        private double _distance = 200000;

        /// <summary>
        /// Название геосервиса
        /// </summary>
        public override string Name => "Here";
        //https://geocoder.ls.hereapi.com/6.2/geocode.json?searchtext=%D0%9D%D0%BE%D0%B2%D0%BE%D1%81%D0%B8%D0%B1%D0%B8%D1%80%D1%81%D0%BA%20&gen=9&apiKey=EXVYrj22JGZG2NN5OO9G9kwfLyyoWrywEEHtJn_8eEw%20%20
        //  public override bool CanUsePolygon => true;

        /// <summary>
        /// Ссылка на геокодер яндекса
        /// </summary>
        protected override string _url => @"https://geocoder.ls.hereapi.com/6.2/geocode.json?searchtext=";

        protected override void ParserJson(Action<IEnumerable<GeoCod>, Exception> callback, string json)
        {
            Exception error = null;
            IEnumerable<GeoCod> data = null;

            try
            {
                Here h = JsonConvert.DeserializeObject<Here>(json);

                if(h.Response.View.Any())
                {
                    var list = h.Response.View[0].Result;

                    data = list.Where(x => x.Distance < _distance).Select(x =>
                    {
                        return GetGeo(x);
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(data, error);
        }

        private GeoCod GetGeo(Data.Result geo)
        {
            var qual = geo.MatchQuality;
            //var street = qual.Street;

            //if (street == null || !street.Any())
            //{
            //    street = new double[1] { -1 };
            //}
            return new GeoCod()
            {
                Kind = geo.MatchLevel,
                Latitude = geo.Location.DisplayPosition.Latitude.ToString(),
                Longitude = geo.Location.DisplayPosition.Longitude.ToString(),
                Text = geo.Location.Address.Label,
                Precision = geo.Relevance.ToString(),
                MatchQuality = $"{qual.Country}|{qual.State}|{qual.District}|{qual.Subdistrict}|{qual.City}|{qual.Street?[0]}|{qual.HouseNumber}|{qual.PostalCode}|{qual.Building}|{geo.Relevance}"
            };
        }

        /// <summary>
        /// Метод для формирования урла с веб запросом
        /// </summary>
        /// <param name="address">Адрес для вебзапроса</param>
        /// <returns>Урл для вебзапроса</returns>
        public override string GetUrlRequest(string address, List<double> polygon)
        {
           // var str = _key.Split(' ');
            var box = string.Empty;

            if (polygon != null && polygon.Count == 4)
            {
                box =  $"&mapview={DoubleToString(polygon[1])}%2C{DoubleToString(polygon[0])}%3B{DoubleToString(polygon[3])}%2C{DoubleToString(polygon[2])}";

                _distance = Distance.DistanceBetweenPlaces(polygon[0], polygon[1], polygon[2], polygon[3])/2;
            }

            return $"{_url}{address}{box}&apiKey={_key}";
        }

        public override void SetKeyApi(string keyApi)
        {
            _key = keyApi;
        }

        public override string GetKeyApi()
        {
            return _key;
        }
    }
}