using System;
using System.Threading;

namespace GeoCoding.GeoCodingService.Test
{
    public class GeoCodingTest : IGeoCodingService
    {
        public void GetGeoCod(Action<GeoCod, Exception> callback, string address)
        {
            Exception error = null;
            Random rnd = new Random();
            string[] kind = new string[] { "House", "Street", "Metro", "District", "Locality" };
            string[] precision = new string[] { "Exact", "Number", "Near", "Range", "Street" };

            GeoCod geocod = new GeoCod()
            {
                CountResult = (byte)rnd.Next(1, 3),
                Latitude = "",
                Longitude = "",
                Text = "",
                Kind = (string)kind.GetValue(rnd.Next(kind.Length)),
                Precision = (string)precision.GetValue(rnd.Next(precision.Length)),
            };
            Thread.Sleep(50);
            callback(geocod, error);
        }
    }
}