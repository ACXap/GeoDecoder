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
        //[JsonProperty("metaDataProperty")]
        //public GeoObjectCollectionMetaDataProperty MetaDataProperty { get; set; }

        [JsonProperty("featureMember")]
        public List<FeatureMember> FeatureMember { get; set; }
    }

    //public partial class GeoObjectCollectionMetaDataProperty
    //{
    //    [JsonProperty("GeocoderResponseMetaData")]
    //    public GeocoderResponseMetaData GeocoderResponseMetaData { get; set; }
    //}

    //public partial class GeocoderResponseMetaData
    //{
    //    [JsonProperty("request")]
    //    public string Request { get; set; }

    //    [JsonProperty("found")]
    //    [JsonConverter(typeof(ParseStringConverter))]
    //    public long Found { get; set; }

    //    [JsonProperty("results")]
    //    [JsonConverter(typeof(ParseStringConverter))]
    //    public long Results { get; set; }
    //}

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
    //internal class ParseStringConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

    //    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.Null) return null;
    //        var value = serializer.Deserialize<string>(reader);
    //        long l;
    //        if (Int64.TryParse(value, out l))
    //        {
    //            return l;
    //        }
    //        throw new Exception("Cannot unmarshal type long");
    //    }

    //    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    //    {
    //        if (untypedValue == null)
    //        {
    //            serializer.Serialize(writer, null);
    //            return;
    //        }
    //        var value = (long)untypedValue;
    //        serializer.Serialize(writer, value.ToString());
    //        return;
    //    }

    //    public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    //}
}