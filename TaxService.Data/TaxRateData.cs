using TaxService.Core.Interfaces;

namespace TaxService.Data
{
    /// <summary>
    /// Implements ITaxRateData defined by core assembly for returning tax data in a common format from different providers
    /// </summary>
    public class TaxRateData : ITaxRateData
    {
        public decimal CityRate { get; set; }
        public decimal CountyRate { get; set; }
        public decimal StateRate { get; set; }
        public decimal TotalRate { get; set; }
    }
}