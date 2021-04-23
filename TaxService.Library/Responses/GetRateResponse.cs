namespace TaxService.Library.Responses
{
    public class GetRateResponse 
    {
        public decimal CityRate { get; set; }
        public decimal CountyRate { get; set; }
        public decimal StateRate { get; set; }
        public decimal TotalRate { get; set; }
    }
}