using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxService.Controllers;
using TaxService.Core;
using TaxService.Core.Interfaces;
using TaxService.Library.Requests;
using TaxService.Library.Responses;

namespace TaxService.Tests
{
    [TestClass]
    public class TaxControllerTests
    {
        [ThreadStatic]
        TaxController Controller;

        [ThreadStatic]
        Mock<ILogger<TaxController>> Logger;

        [ThreadStatic]
        Mock<ITaxServiceProcessor> Service;

        [TestInitialize]
        public void TestInitialize()
        {
            Service = new Mock<ITaxServiceProcessor>();
            Logger = new Mock<ILogger<TaxController>>();
            Controller = new TaxController(Logger.Object, Service.Object);
        }

        [TestMethod]
        public void CalculateTax_Should_LogExceptions_When_Thrown()
        {
            var ex = new Exception("La dee da, la dee da");
            Service.Setup(s => s.CalculateTax(It.IsAny<CalculateTaxRequest>()))
                   .Throws(ex);

            var request = new CalculateTaxRequest { State = "NE", ZipPostalCode = "HALL" };
            var result = Controller.CalculateTax(request).Result as StatusCodeResult;

            result.Should().NotBeNull();
            Logger.Verify(l => l.Log(It.Is<LogLevel>(l => l == LogLevel.Error),
                                     It.IsAny<EventId>(),
                                     It.Is<It.IsAnyType>((v, t) => true),
                                     It.Is<Exception>(e => e == ex),
                                     It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                          Times.Once);
        }

        [TestMethod]
        public void CalculateTax_Should_ReturnBadRequest_When_ServiceDoesNotReturnSuccess()
        {
            var error = new ErrorResponse("ABC", "Easy as 1, 2, 3");
            var error2 = new ErrorResponse("DOREMI", "A, B, C. 1, 2, 3");
            var response = new ProcessorResponse(new[] { error, error2 });

            Service.Setup(s => s.CalculateTax(It.IsAny<CalculateTaxRequest>()))
                   .Returns(Task.FromResult(response));

            var request = new CalculateTaxRequest { ZipPostalCode = "ABC123", State = "ME" };
            var result = Controller.CalculateTax(request).Result as BadRequestObjectResult;
            result.Should().NotBeNull();

            var errors = result.Value as IEnumerable<ErrorResponse>;
            errors?.Count(e => e.Code == error.Code &&
                               e.Description == error.Description)
                   .Should().Be(1);
            errors?.Count(e => e.Code == error2.Code &&
                               e.Description == error2.Description)
                   .Should().Be(1);
        }

        [TestMethod]
        public void CalculateTax_Should_ReturnBadRequest_When_StateIsNotSupplied()
        {
            var expectedError = string.Format(BaseController.ErrorDescriptions.MissingRequiredField, nameof(CalculateTaxRequest.State));

            //Null object should get us the expected error 
            var result = Controller.CalculateTax(null).Result as BadRequestObjectResult;
            result.Should().NotBeNull();

            var errors = result.Value as IEnumerable<ErrorResponse>;
            errors?.Count(e => e.Code == ErrorResponse.Codes.MissingRequiredField &&
                               e.Description == expectedError)
                   .Should().Be(1);

            //Repeat for an instance of the object, but with the default null value
            var request = new CalculateTaxRequest { ZipPostalCode = "ABC123" };
            result = Controller.CalculateTax(request).Result as BadRequestObjectResult;
            result.Should().NotBeNull();

            errors = result.Value as IEnumerable<ErrorResponse>;
            errors?.Count(e => e.Code == ErrorResponse.Codes.MissingRequiredField &&
                               e.Description == expectedError)
                   .Should().Be(1);

            //One more time with a blank string
            request.State = string.Empty;
            result = Controller.CalculateTax(request).Result as BadRequestObjectResult;
            result.Should().NotBeNull();

            errors = result.Value as IEnumerable<ErrorResponse>;
            errors?.Count(e => e.Code == ErrorResponse.Codes.MissingRequiredField &&
                               e.Description == expectedError)
                   .Should().Be(1);

            //And lastly a bunch of whitespace
            request.State = "       ";
            result = Controller.CalculateTax(request).Result as BadRequestObjectResult;
            result.Should().NotBeNull();

            errors = result.Value as IEnumerable<ErrorResponse>;
            errors?.Count(e => e.Code == ErrorResponse.Codes.MissingRequiredField &&
                               e.Description == expectedError)
                   .Should().Be(1);

            //At no point was the method on the service ever even invoked
            Service.Verify(s => s.CalculateTax(It.IsAny<CalculateTaxRequest>()), Times.Never);
        }

        [TestMethod]
        public void CalculateTax_Should_ReturnBadRequest_When_ZipPostalCodeIsNotSupplied()
        {
            var expectedError = string.Format(BaseController.ErrorDescriptions.MissingRequiredField, nameof(CalculateTaxRequest.ZipPostalCode));

            //Null object should get us the expected error 
            var result = Controller.CalculateTax(null).Result as BadRequestObjectResult;
            result.Should().NotBeNull();
            
            var errors = result.Value as IEnumerable<ErrorResponse>;
            errors?.Count(e => e.Code == ErrorResponse.Codes.MissingRequiredField &&
                               e.Description == expectedError)
                   .Should().Be(1);

            //Repeat for an instance of the object, but with the default null value
            var request = new CalculateTaxRequest();
            result = Controller.CalculateTax(request).Result as BadRequestObjectResult;
            result.Should().NotBeNull();

            errors = result.Value as IEnumerable<ErrorResponse>;
            errors?.Count(e => e.Code == ErrorResponse.Codes.MissingRequiredField &&
                               e.Description == expectedError)
                   .Should().Be(1);

            //One more time with a blank string
            request.ZipPostalCode = string.Empty;
            result = Controller.CalculateTax(request).Result as BadRequestObjectResult;
            result.Should().NotBeNull();

            errors = result.Value as IEnumerable<ErrorResponse>;
            errors?.Count(e => e.Code == ErrorResponse.Codes.MissingRequiredField &&
                               e.Description == expectedError)
                   .Should().Be(1);

            //And lastly a bunch of whitespace
            request.ZipPostalCode = "       ";
            result = Controller.CalculateTax(request).Result as BadRequestObjectResult;
            result.Should().NotBeNull();

            errors = result.Value as IEnumerable<ErrorResponse>;
            errors?.Count(e => e.Code == ErrorResponse.Codes.MissingRequiredField &&
                               e.Description == expectedError)
                   .Should().Be(1);

            //At no point was the method on the service ever even invoked
            Service.Verify(s => s.CalculateTax(It.IsAny<CalculateTaxRequest>()), Times.Never);
        }

        [TestMethod]
        public void CalculateTax_Should_ReturnOk_When_ServiceReturnsSuccess()
        {
            var response = new ProcessorResponse(new CalculateTaxResponse { TotalTax = 123.456m });

            Service.Setup(s => s.CalculateTax(It.IsAny<CalculateTaxRequest>()))
                   .Returns(Task.FromResult(response));

            var request = new CalculateTaxRequest { State = "OK", ZipPostalCode = "SUCCESS" };
            var result = Controller.CalculateTax(request).Result as OkObjectResult;
            result.Should().NotBeNull();

            var returnedTax = result.Value as CalculateTaxResponse;
            returnedTax.Should().Be(response.ResponseData);
        }
    }
}