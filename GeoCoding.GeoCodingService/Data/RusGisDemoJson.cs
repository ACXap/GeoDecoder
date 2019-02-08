namespace GeoCoding.GeoCodingService
{
    public class RusGisDemoJson
    {
        public string id { get; set; }
        public string requestAddress { get; set; }
        public string text { get; set; }
        public string kind { get; set; }
        public string precision { get; set; }
        public object description { get; set; }
        public string name { get; set; }
        public double lowerCornerX { get; set; }
        public double lowerCornerY { get; set; }
        public double upperCornerX { get; set; }
        public double upperCornerY { get; set; }
        public double posX { get; set; }
        public double posY { get; set; }
        public string geom { get; set; }
        public object requestCoords { get; set; }
    }
}