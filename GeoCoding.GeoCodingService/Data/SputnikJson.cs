using System.Collections.Generic;

namespace GeoCoding.GeoCodingService
{
        public class Meta
        {
            public string version { get; set; }
            public string format { get; set; }
        }

        public class Viewport
        {
            public double TopLat { get; set; }
            public double TopLon { get; set; }
            public double BotLat { get; set; }
            public double BotLon { get; set; }
        }

        public class AddressComponent
        {
            public string type { get; set; }
            public string value { get; set; }
        }

        public class Properties
        {
            public int id { get; set; }
            public string type { get; set; }
            public string description { get; set; }
            public string display_name { get; set; }
            public string title { get; set; }
            public List<AddressComponent> address_components { get; set; }
            public string fias_id { get; set; }
            public bool full_match { get; set; }
            public object poi_types { get; set; }
        }

        public class Geometry2
        {
            public string type { get; set; }
            public List<double> coordinates { get; set; }
        }

        public class Geometry
        {
            public string type { get; set; }
            public List<Geometry2> geometries { get; set; }
        }

        public class Feature
        {
            public string type { get; set; }
            public Properties properties { get; set; }
            public Geometry geometry { get; set; }
        }

        public class Address
        {
            public string type { get; set; }
            public List<Feature> features { get; set; }
        }

        public class Result
        {
            public string priority { get; set; }
            public Viewport viewport { get; set; }
            public List<Address> address { get; set; }
        }

        public class Typo
        {
            public string OriginalQuery { get; set; }
            public string FixedQuery { get; set; }
            public int Rank { get; set; }
        }

        public class SputnikJsonRootObject
        {
            public Meta meta { get; set; }
            public Result result { get; set; }
            public Typo typo { get; set; }
        }
}