using Newtonsoft.Json;

namespace TaxService.Data.TaxJar
{
    /// <summary>
    /// Response object defined by TaxJar for their responses endpoint
    /// </summary>
    internal class TaxResponse
    {
        [JsonProperty("tax")]
        public TaxDetail Tax { get; set; }
    }

    internal class TaxDetail
    {
        [JsonProperty("amount_to_collect")]
        public float TaxAmount { get; set; }
    }
}
