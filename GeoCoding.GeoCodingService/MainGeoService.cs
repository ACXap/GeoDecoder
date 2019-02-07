using System.Collections.Generic;

namespace GeoCoding.GeoCodingService
{
    public static class MainGeoService
    {
        public static IEnumerable<GeoService> GetAllService()
        {
            return new List<GeoService>()
            {
                new GeoService()
                {
                    Name = "Yandex",
                    Service = new YandexGeoCodingService()
                },
                new GeoService()
                {
                    Name = "Sputnik",
                    Service = new SputnikGeoCodingService()
                }
            };
        }
    }
}