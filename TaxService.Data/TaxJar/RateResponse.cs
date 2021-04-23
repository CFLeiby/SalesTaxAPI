using Newtonsoft.Json;

namespace TaxService.Data.TaxJar
{
    /// <summary>
    /// Response object defined by TaxJar for their rate/get endpoint
    /// </summary>
    internal class RateResponse
    {
        [JsonProperty("rate")]
        public RateDetail Rate { get; set; }
    }

    internal class RateDetail
    {
        [JsonProperty("city_rate")]
        public string CityRate { get; set; }

        [JsonProperty("combined_rate")]
        public string CombinedRate { get; set; }

        [JsonProperty("county_rate")]
        public string CountyRate { get; set; }

        [JsonProperty("state_rate")]
        public string StateRate { get; set; }
    }
}