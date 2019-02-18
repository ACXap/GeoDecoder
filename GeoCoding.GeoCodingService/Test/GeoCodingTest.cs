﻿using System;
using System.Collections.Generic;

namespace GeoCoding.GeoCodingService.Test
{
    /// <summary>
    /// Тестовый геокодер, просто рандом
    /// </summary>
    public class GeoCodingTest : IGeoCodingService
    {
        private readonly Random _rnd = new Random();
        public string Name => "Test";

        public void GetGeoCod(Action<IEnumerable<GeoCod>, Exception> callback, string address)
        {
            Exception error = null;
            string[] kind = new string[] { "House", "Street", "Metro", "District", "Locality" };
            string[] precision = new string[] { "Exact", "Number", "Near", "Range", "Street" };

            List<GeoCod> data = new List<GeoCod>()
            {
                 new GeoCod()
                 {
                    CountResult = _rnd.Next(0, 5),
                    Latitude = $"{_rnd.Next(0,120)}.{_rnd.Next(0,900000)}",
                    Longitude = $"{_rnd.Next(0, 120)}.{_rnd.Next(0, 900000)}",
                    Text = "",
                    Kind = (string)kind.GetValue(_rnd.Next(kind.Length)),
                    Precision = (string)precision.GetValue(_rnd.Next(precision.Length)),
                 },
                 new GeoCod()
                 {
                    CountResult = _rnd.Next(0, 5),
                    Latitude = $"{_rnd.Next(0,120)}.{_rnd.Next(0,900000)}",
                    Longitude = $"{_rnd.Next(0, 120)}.{_rnd.Next(0, 900000)}",
                    Text = "",
                    Kind = (string)kind.GetValue(_rnd.Next(kind.Length)),
                    Precision = (string)precision.GetValue(_rnd.Next(precision.Length)),
                 },
                 new GeoCod()
                 {
                    CountResult = _rnd.Next(0, 5),
                    Latitude = $"{_rnd.Next(0,120)}.{_rnd.Next(0,900000)}",
                    Longitude = $"{_rnd.Next(0, 120)}.{_rnd.Next(0, 900000)}",
                    Text = "",
                    Kind = (string)kind.GetValue(_rnd.Next(kind.Length)),
                    Precision = (string)precision.GetValue(_rnd.Next(precision.Length)),
                 }
            };

            callback(data, error);
        }

        public string GetUrlRequest(string address)
        {
            return null;
        }
    }
}