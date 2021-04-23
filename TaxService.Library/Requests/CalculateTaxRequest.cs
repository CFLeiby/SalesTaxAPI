namespace TaxService.Library.Requests
{
    public class CalculateTaxRequest
    {
        public string State { get; set; }
        public decimal TaxableAmount { get; set; }
        public string ZipPostalCode { get; set; }
    }
}