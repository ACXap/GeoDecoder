namespace GeoCoding.GeoCodingService
{
    public class GeoService
    {
        public string Name { get; set; }
        public IGeoCodingService Service { get; set; }
    }
}