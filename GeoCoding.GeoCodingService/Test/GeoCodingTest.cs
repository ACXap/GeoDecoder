using System;
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

        public void GetGeoCod(Action<IEnumerable<GeoCod>, Exception> callback, string address, ConnectSettings cs = null)
        {
            Exception error = null;
            string[] kind = new string[] { "House", "Street", "Metro", "District", "Locality" };
            string[] precision = new string[] { "Exact", "Number", "Near", "Range", "Street" };

            List<GeoCod> data = null;

            if (_rnd.Next(0, 2) == 1)
            {
                data = new List<GeoCod>();
                int count = _rnd.Next(0, 6);
                for (int i = 0; i < count; i++)
                {
                    data.Add(new GeoCod()
                    {
                        Latitude = $"{_rnd.Next(0, 120)}.{_rnd.Next(0, 900000)}",
                        Longitude = $"{_rnd.Next(0, 120)}.{_rnd.Next(0, 900000)}",
                        Text = "",
                        Kind = (string)kind.GetValue(_rnd.Next(kind.Length)),
                        Precision = (string)precision.GetValue(_rnd.Next(precision.Length)),
                    });
                }

            }
            else if (_rnd.Next(0, 5) == 3)
            {
                error = new ArgumentException("Big Error");
            }

            callback(data, error);
        }

        public string GetUrlRequest(string address)
        {
            return null;
        }
    }
}