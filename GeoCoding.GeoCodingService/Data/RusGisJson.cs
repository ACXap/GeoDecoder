// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Newtonsoft.Json;

namespace GeoCoding.GeoCodingService
{
    public class RusGisJson
    {
        [JsonProperty("requestAddress")]
        public string RequestAddress { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("precision")]
        public string Precision { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lowerCornerX")]
        public double LowerCornerX { get; set; }

        [JsonProperty("lowerCornerY")]
        public double LowerCornerY { get; set; }

        [JsonProperty("upperCornerX")]
        public double UpperCornerX { get; set; }

        [JsonProperty("upperCornerY")]
        public double UpperCornerY { get; set; }

        [JsonProperty("posX")]
        public double PosX { get; set; }

        [JsonProperty("posY")]
        public double PosY { get; set; }

        [JsonProperty("geom")]
        public object Geom { get; set; }

        [JsonProperty("requestCoords")]
        public object RequestCoords { get; set; }
    }
}