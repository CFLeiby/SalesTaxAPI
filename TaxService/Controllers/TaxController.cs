using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxService.Core.Interfaces;
using TaxService.Library.Requests;
using TaxService.Library.Responses;

namespace TaxService.Controllers
{
    /// <summary>
    /// Contains endpoints related to tax calculations
    /// </summary>
    [ApiController]
    [Route("tax")]
    public class TaxController : BaseController
    {
        public TaxController(ILogger<TaxController> logger, ITaxServiceProcessor service)
            : base(logger, service)
        { }

        protected override string ControllerName => "TaxController";

        /// <summary>
        /// Retruns the tax on a given amount for a given zip code
        /// </summary>
        [HttpPost]
        [Route("calculate")]
        public async Task<IActionResult> CalculateTax(CalculateTaxRequest request)
        {
            const string name = "GetRate";

            try
            {
                LogEntry(name, request);

                var errors = new List<ErrorResponse>();
                //State is required
                if (string.IsNullOrWhiteSpace(request?.State))
                {
                    var msg = string.Format(ErrorDescriptions.MissingRequiredField, nameof(CalculateTaxRequest.State));
                    var error = new ErrorResponse(ErrorResponse.Codes.MissingRequiredField, msg);
                    errors.Add(error);
                }
                //As is Zip
                if (string.IsNullOrWhiteSpace(request?.ZipPostalCode))
                {
                    var msg = string.Format(ErrorDescriptions.MissingRequiredField, nameof(CalculateTaxRequest.ZipPostalCode));
                    var error = new ErrorResponse(ErrorResponse.Codes.MissingRequiredField, msg);
                    errors.Add(error);
                }
                if (errors.Any())
                    return BadRequest(errors);

                var result = await _service.CalculateTax(request);
                return ProcessServiceResponse(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(name, ex);
            }
        }
    }
}