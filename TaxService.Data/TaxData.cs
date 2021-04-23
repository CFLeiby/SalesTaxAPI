using TaxService.Core.Interfaces;

namespace TaxService.Data
{
    /// <summary>
    /// Implements ITaxData defined by core assembly for returning tax data in a common format from different providers
    /// </summary>
    public class TaxData : ITaxData
    {
        public decimal TotalTax { get; set; }
    }
}