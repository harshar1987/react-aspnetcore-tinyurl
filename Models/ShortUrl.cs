using System;
using Newtonsoft.Json;

namespace UrlShortnerApi.Models
{
    public class ShortUrl
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "original_url")]
        public string OriginalUrl { get; set; }

        [JsonProperty(PropertyName = "created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "updated_date")]
        public DateTime UpdatedDate { get; set; }
    }
}