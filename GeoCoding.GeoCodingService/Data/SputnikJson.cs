using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GeoCoding.GeoCodingService
{
    public class Meta
    {
        [JsonProperty("found")]
        public long Found { get; set; }

        [JsonProperty("shown")]
        public long Shown { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }

    public class Position
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }
    }

    public class Result
    {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("display_name")]
            public string DisplayName { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("fias_id", NullValueHandling = NullValueHandling.Ignore)]
            public Guid? FiasId { get; set; }

            [JsonProperty("SortScore")]
            public double SortScore { get; set; }

            [JsonProperty("full_match")]
            public bool FullMatch { get; set; }

            [JsonProperty("position")]
            public Position Position { get; set; }
    }

    public class Typo
    {
        [JsonProperty("OriginalQuery")]
        public string OriginalQuery { get; set; }

        [JsonProperty("FixedQuery")]
        public string FixedQuery { get; set; }

        [JsonProperty("Rank")]
        public long Rank { get; set; }
    }

    public class SputnikJson
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("result")]
        public List<Result> Result { get; set; }

        [JsonProperty("typo")]
        public Typo Typo { get; set; }
    }
}