using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TaxService.Core.Interfaces;
using TaxService.Library.Requests;
using TaxService.Library.Responses;

namespace TaxService.Controllers
{
    /// <summary>
    /// Contains endpoints related to tax rates
    /// </summary>
    [ApiController]
    [Route("rate")]
    public class RateController : BaseController
    {
        public RateController(ILogger<RateController> logger, ITaxServiceProcessor service)
            : base(logger, service)
        { }

        protected override string ControllerName => "RateController";

        /// <summary>
        /// Returns tax rate for a given zip code
        /// </summary>
        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> GetRate(GetRateRequest request)
        {
            const string name = "RateController.GetRate";

            try
            {
                LogEntry(name, request);

                if (string.IsNullOrWhiteSpace(request?.ZipPostalCode))
                {
                    var msg = string.Format(ErrorDescriptions.MissingRequiredField, nameof(GetRateRequest.ZipPostalCode));
                    var error = new ErrorResponse(ErrorResponse.Codes.MissingRequiredField, msg);
                    return BadRequest(new[] { error });
                }

                var result = await _service.GetRate(request);
                return ProcessServiceResponse(result);
            }
            catch (Exception ex)
            {
                return HandleControllerException(name, ex);
            }
        }
    }
}