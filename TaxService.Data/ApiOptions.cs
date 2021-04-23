namespace TaxService.Data
{
    /// <summary>
    /// Holds required values for connecting to an API as defined in appsettings
    /// </summary>
    public class ApiOptions
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
    }
}
