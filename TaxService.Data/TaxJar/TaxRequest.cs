using Newtonsoft.Json;

namespace TaxService.Data.TaxJar
{
    /// <summary>
    /// Request object defined by TaxJar for their taxes endpoint
    /// </summary>
    internal class TaxRequest
    {
        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("to_state")]
        public string DeliveryState { get; set; }

        [JsonProperty("to_zip")]
        public string DeliveryZipPostalCode { get; set; }

        [JsonProperty("from_state")]
        public string SourceState => DeliveryState;

        [JsonProperty("from_zip")]
        public string SourceZipPostalCode => DeliveryZipPostalCode;

        [JsonProperty("shipping")]
        public static float ShippingAmount => 0;
    }
}