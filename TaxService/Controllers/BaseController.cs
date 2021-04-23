using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using TaxService.Core;
using TaxService.Core.Interfaces;

[assembly: InternalsVisibleTo("TaxService.Tests")]
namespace TaxService.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        internal static class ErrorDescriptions
        {
            public const string MissingRequiredField = "{0} is required.";
            public const string UnexpectedError = "An unexpected error has occurred.  Your request could not be completed at this time.";
        }

        private readonly ILogger _logger;
        protected readonly ITaxServiceProcessor _service;

        protected BaseController(ILogger logger, ITaxServiceProcessor service)
        {
            _logger = logger;
            _service = service;
        }

        protected IActionResult HandleControllerException(string methodName, Exception ex)
        {
            _logger.LogError(ex, $"{ControllerName}.{methodName} threw exception.");
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }

        protected void LogEntry(string methodName, object requestData)
        {
            _logger.LogInformation($"TaxController.{methodName}: executing for {JsonConvert.SerializeObject(requestData)}");
        }

        protected IActionResult ProcessServiceResponse(ProcessorResponse response)
        {
            if (response.Success)
                return Ok(response.ResponseData);

            return BadRequest(response.Errors);
        }

        protected abstract string ControllerName { get; }
    }
}
