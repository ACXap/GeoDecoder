using Newtonsoft.Json;
using System;

namespace GeoCoding.GeoCodingService
{
    public class HereJson
    {
        [JsonProperty("requestAddress")]
        public string RequestAddress { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("precision")]
        public string Precision { get; set; }

        [JsonProperty("description"), JsonConverter(typeof(DescriptionConverter))]
        public Description Description { get; set; }

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

    public class Description
    {
        public double HouseNumber { get; set; }
        public double Street { get; set; }
        public double PostalCode { get; set; }
        public double City { get; set; }
        public double County { get; set; }
        public double District { get; set; }
        public double State { get; set; }
        public double Subdistrict { get; set; }
        public double Building { get; set; }
    }

    public class DescriptionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader != null && string.IsNullOrEmpty((string)reader.Value))
            {
                return null;
            }
            try
            {
                var desc = (string)reader.Value;
                var res = desc.Replace(@"\", "").Replace("[", "").Replace("]", "");
                var a = JsonConvert.DeserializeObject<Description>(res);
                return a;
            }
            catch
            {
                throw;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}