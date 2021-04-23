using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaxService.Core.Interfaces;
using TaxService.Library.Requests;
using TaxService.Library.Responses;

namespace TaxService.Core
{
    /// <summary>
    /// Contains core business logic supporting all controller endpoints.  All
    /// methods return a common ServiceResponse object to standardize returing
    /// either response data or errors encountered during validation and/or processing
    /// </summary>
    public class TaxServiceProcessor : ITaxServiceProcessor
    {
        #region Constants

        /// <summary>
        /// As we add support for additional tax providers, we'll want to add entries here
        /// so that they can be properly registered and retrieved as requested
        /// </summary>
        public enum DataProviders
        {
            TaxJar
        }

        private static readonly ErrorResponse _providerUnavailableError = new(ErrorResponse.Codes.TaxProviderUnavailable,
            "The requested tax provider is not available.");

        #endregion Constants

        #region Fields

        private readonly ILogger _logger;
        private readonly IEnumerable<IDataProvider> _providers;

        #endregion Fields

        #region Constructors

        public TaxServiceProcessor(IEnumerable<IDataProvider> providers, ILogger<TaxServiceProcessor> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Requests the appropriate provider to calculate the tax for the supplied request
        /// </summary>
        public async Task<ProcessorResponse> CalculateTax(CalculateTaxRequest request)
        {
            var provider = SelectProvider();
            if (provider == null)
                return new ProcessorResponse(new[] { _providerUnavailableError });

            var data = await provider.CalculateTax(request);
            if (data == null)
                return new ProcessorResponse(data);

            var returnObject = new CalculateTaxResponse { TotalTax = data.TotalTax };
            return new ProcessorResponse(returnObject);
        }

        /// <summary>
        /// Requests the appropriate provider to return the tax rate for the supplied request
        /// </summary>
        public async Task<ProcessorResponse> GetRate(GetRateRequest request)
        {
            var provider = SelectProvider();
            if (provider == null)
                return new ProcessorResponse(new[] { _providerUnavailableError });

            ITaxRateData data = await provider.GetRate(request);
            if (data == null)
                return new ProcessorResponse(data);

            var returnObject = new GetRateResponse
            {
                CityRate = data.CityRate,
                CountyRate = data.CountyRate,
                StateRate = data.StateRate,
                TotalRate = data.TotalRate
            };
            return new ProcessorResponse(returnObject);
        }

        internal IDataProvider SelectProvider()
        {
            //if/when we have other providers, this is where we'll figure out which of the configured one's to return
            var provider = _providers?.FirstOrDefault(p => p.ProviderType == DataProviders.TaxJar);
            if (provider == null)
                _logger.LogWarning("TaxService: Valid provider is not configured.");

            return provider;
        }

        #endregion Methods
    }
}
