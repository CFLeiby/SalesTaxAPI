using System.Collections.Generic;
using System.Linq;
using TaxService.Library.Responses;

namespace TaxService.Core
{
    /// <summary>
    /// Common response of service methods, this will indicate whether or not the result was
    /// a success and contain either the object to be returned to the client (if success is true)
    /// or a list of any errors encountered (if not)
    /// </summary>
    public class ProcessorResponse
    {
        public ProcessorResponse(object responseData)
        {
            Errors = new List<ErrorResponse>();
            ResponseData = responseData;
            Success = true;
        }

        public ProcessorResponse(IEnumerable<ErrorResponse> errors)
        {
            Errors = errors.ToList();
            Success = false;
        }

        public IList<ErrorResponse> Errors { get; private set; }
        public object ResponseData { get; private set; }
        public bool Success { get; set; }
    }
}
