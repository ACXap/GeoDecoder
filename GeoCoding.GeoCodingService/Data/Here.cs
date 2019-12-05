// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.GeoCodingService.Data
{
    public partial class Here
    {
        [JsonProperty("Response")]
        public Response Response { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("MetaInfo")]
        public MetaInfo MetaInfo { get; set; }

        [JsonProperty("View")]
        public View[] View { get; set; }
    }

    public partial class MetaInfo
    {
        [JsonProperty("Timestamp")]
        public string Timestamp { get; set; }
    }

    public partial class View
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("ViewId")]
        public long ViewId { get; set; }

        [JsonProperty("Result")]
        public Result[] Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("Relevance")]
        public double Relevance { get; set; }

        [JsonProperty("Distance")]
        public double Distance { get; set; }

        [JsonProperty("MatchLevel")]
        public string MatchLevel { get; set; }

        [JsonProperty("MatchQuality")]
        public MatchQuality MatchQuality { get; set; }

        [JsonProperty("MatchType")]
        public string MatchType { get; set; }

        [JsonProperty("Location")]
        public Location Location { get; set; }
    }

    public partial class Location
    {
     //   [JsonProperty("LocationId")]
     //   public string LocationId { get; set; }

     //   [JsonProperty("LocationType")]
     //   public string LocationType { get; set; }

        [JsonProperty("DisplayPosition")]
        public DisplayPosition DisplayPosition { get; set; }

      //  [JsonProperty("NavigationPosition")]
     //   public DisplayPosition[] NavigationPosition { get; set; }

       // [JsonProperty("MapView")]
      //  public MapView MapView { get; set; }

        [JsonProperty("Address")]
        public Address Address { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("Label")]
        public string Label { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

        [JsonProperty("County")]
        public string County { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("District")]
        public string District { get; set; }

        [JsonProperty("Street")]
        public string Street { get; set; }

        [JsonProperty("HouseNumber")]
       // [JsonConverter(typeof(ParseStringConverter))]
        public string HouseNumber { get; set; }

     //   [JsonProperty("PostalCode")]
      //  [JsonConverter(typeof(ParseStringConverter))]
      //  public long PostalCode { get; set; }

      //  [JsonProperty("AdditionalData")]
      //  public AdditionalDatum[] AdditionalData { get; set; }
    }

    public partial class AdditionalDatum
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }

    public partial class DisplayPosition
    {
        [JsonProperty("Latitude")]
        public double Latitude { get; set; }

        [JsonProperty("Longitude")]
        public double Longitude { get; set; }
    }

    public partial class MapView
    {
        [JsonProperty("TopLeft")]
        public DisplayPosition TopLeft { get; set; }

        [JsonProperty("BottomRight")]
        public DisplayPosition BottomRight { get; set; }
    }

    public partial class MatchQuality
    {
        [JsonProperty("Country")]
        public double? Country { get; set; }

        [JsonProperty("State")]
        public double? State { get; set; }

        [JsonProperty("District")]
        public double? District { get; set; }

        [JsonProperty("Subdistrict")]
        public double? Subdistrict { get; set; }

        [JsonProperty("City")]
        public double? City { get; set; }

        [JsonProperty("Street")]
        public double?[] Street { get; set; }

        [JsonProperty("HouseNumber")]
        public double? HouseNumber { get; set; }

        [JsonProperty("PostalCode")]
        public double? PostalCode { get; set; }

        [JsonProperty("Building")]
        public double? Building { get; set; }
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
