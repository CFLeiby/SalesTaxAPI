namespace TaxService.Library.Responses
{
    public class ErrorResponse
    {
        public static class Codes
        {
            public const string UnexpectedError = "1000";
            public const string MissingRequiredField = "1001";
            public const string TaxProviderUnavailable = "1002";
        }

        public ErrorResponse()
        { }

        public ErrorResponse(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; set; }
        public string Description { get; set; }
    }
}
