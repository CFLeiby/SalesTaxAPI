using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxService.Core.Interfaces;
using TaxService.Data;
using TaxService.Library.Requests;
using TaxService.Library.Responses;

namespace TaxService.Core.Tests
{
    [TestClass]
    public class TaxServiceProcessorTests
    {
        [ThreadStatic]
        Mock<ILogger<TaxServiceProcessor>> Logger;

        [ThreadStatic]
        List<Mock<IDataProvider>> Providers;

        [TestInitialize]
        public void TestInitialize()
        {
            Logger = new Mock<ILogger<TaxServiceProcessor>>();
            var provider = new Mock<IDataProvider>();
            provider.SetupAllProperties();
            Providers = new List<Mock<IDataProvider>>() { provider };
        }

        [TestMethod]
        public void CalculateTax_Should_ReturnError_When_ProviderIsUnvailable()
        {
            var service = new TaxServiceProcessor(null, Logger.Object);
            var request = new CalculateTaxRequest();
            var result = service.CalculateTax(request).Result;
            result.Success.Should().BeFalse();
            result.Errors.Count(e => e.Code == ErrorResponse.Codes.TaxProviderUnavailable).Should().Be(1);
        }

        [TestMethod]
        public void CalculateTax_Should_ReturnNullResponseData_When_ProviderReturnsNull()
        {
            var service = new TaxServiceProcessor(Providers.Select(p => p.Object), Logger.Object);
            var request = new CalculateTaxRequest();
            var result = service.CalculateTax(request).Result;
            result.Success.Should().BeTrue();
            result.ResponseData.Should().BeNull();
        }

        [TestMethod]
        public void CalculateTax_Should_ReturnProperlyPopulatedResponseData()
        {
            ITaxData expectedData = new TaxData { TotalTax = 12345678.90m };
            Providers[0].Setup(p => p.CalculateTax(It.IsAny<CalculateTaxRequest>()))
                        .Returns(Task.FromResult(expectedData));

            var service = new TaxServiceProcessor(Providers.Select(p => p.Object), Logger.Object);
            var request = new CalculateTaxRequest();
            var result = service.CalculateTax(request).Result;
            result.Success.Should().BeTrue();

            var response = result.ResponseData as CalculateTaxResponse;
            response.Should().NotBeNull();
            response.TotalTax.Should().Be(expectedData.TotalTax);
        }

        [TestMethod]
        public void GetRate_Should_ReturnErrorWhenProviderIsUnvailable()
        {
            var service = new TaxServiceProcessor(null, Logger.Object);
            var request = new GetRateRequest();
            var result = service.GetRate(request).Result;
            result.Success.Should().BeFalse();
            result.Errors.Count(e => e.Code == ErrorResponse.Codes.TaxProviderUnavailable).Should().Be(1);
        }

        [TestMethod]
        public void GetRate_Should_ReturnNullResponseData_When_ProviderReturnsNull()
        {
            var service = new TaxServiceProcessor(Providers.Select(p => p.Object), Logger.Object);
            var request = new GetRateRequest();
            var result = service.GetRate(request).Result;
            result.Success.Should().BeTrue();
            result.ResponseData.Should().BeNull();
        }

        [TestMethod]
        public void GetRate_Should_ReturnProperlyPopulatedResponseData()
        {
            ITaxRateData expectedData = new TaxRateData { CityRate = 1.23m, CountyRate = 0.45m, StateRate = 0.067m, TotalRate = 8.90m };
            Providers[0].Setup(p => p.GetRate(It.IsAny<GetRateRequest>()))
                        .Returns(Task.FromResult(expectedData));

            var service = new TaxServiceProcessor(Providers.Select(p => p.Object), Logger.Object);
            var request = new GetRateRequest();
            var result = service.GetRate(request).Result;
            result.Success.Should().BeTrue();

            var response = result.ResponseData as GetRateResponse;
            response.Should().NotBeNull();
            response.CityRate.Should().Be(expectedData.CityRate);
            response.CountyRate.Should().Be(expectedData.CountyRate);
            response.StateRate.Should().Be(expectedData.StateRate);
            response.TotalRate.Should().Be(expectedData.TotalRate);
        }
    }
}
