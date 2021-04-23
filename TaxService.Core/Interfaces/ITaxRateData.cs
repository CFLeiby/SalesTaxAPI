namespace TaxService.Core.Interfaces
{
    /// <summary>
    /// Interface that must be implemented by any class supplying data for a TaxRateModel
    /// </summary>
    public interface ITaxRateData
    {
        public decimal CityRate { get; set; }
        public decimal CountyRate { get; set; }
        public decimal StateRate { get; set; }
        public decimal TotalRate { get; set; }
    }
}