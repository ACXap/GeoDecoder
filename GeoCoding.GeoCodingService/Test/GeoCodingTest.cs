using System;
using System.Threading;

namespace GeoCoding.GeoCodingService.Test
{
    /// <summary>
    /// Тестовый геокодер, просто рандом
    /// </summary>
    public class GeoCodingTest : IGeoCodingService
    {
        public string Name => "Test";

        public void GetGeoCod(Action<GeoCod, Exception> callback, string address)
        {
            Exception error = null;
            Random rnd = new Random();
            string[] kind = new string[] { "House", "Street", "Metro", "District", "Locality" };
            string[] precision = new string[] { "Exact", "Number", "Near", "Range", "Street" };

            GeoCod geocod = new GeoCod()
            {
                CountResult = (byte)rnd.Next(0, 5),
                Latitude = $"{rnd.Next(0,120)}.{rnd.Next(0,900000)}",
                Longitude = $"{rnd.Next(0, 120)}.{rnd.Next(0, 900000)}",
                Text = "",
                Kind = (string)kind.GetValue(rnd.Next(kind.Length)),
                Precision = (string)precision.GetValue(rnd.Next(precision.Length)),
            };
            Thread.Sleep(50);
            callback(geocod, error);
        }

        public string GetUrlRequest(string address)
        {
            return null;
        }
    }
}