// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GeoCoding.GeoCodingService
{
    public partial class YandexJson
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }
    public partial class Response
    {
        [JsonProperty("GeoObjectCollection")]
        public GeoObjectCollection GeoObjectCollection { get; set; }
    }
    public partial class GeoObjectCollection
    {
        [JsonProperty("featureMember")]
        public IEnumerable<FeatureMember> FeatureMember { get; set; }
    }

    public partial class FeatureMember
    {
        [JsonProperty("GeoObject")]
        public GeoObject GeoObject { get; set; }
    }
    public partial class GeoObject
    {
        [JsonProperty("metaDataProperty")]
        public GeoObjectMetaDataProperty MetaDataProperty { get; set; }

        [JsonProperty("Point")]
        public Point Point { get; set; }
    }
    public partial class GeoObjectMetaDataProperty
    {
        [JsonProperty("GeocoderMetaData")]
        public GeocoderMetaData GeocoderMetaData { get; set; }
    }

    public partial class GeocoderMetaData
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("precision")]
        public string Precision { get; set; }
    }
    public partial class Point
    {
        [JsonProperty("pos")]
        public string Pos { get; set; }
    }
}