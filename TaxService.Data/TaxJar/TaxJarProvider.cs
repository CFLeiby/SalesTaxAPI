using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TaxService.Core.Interfaces;
using TaxService.Library.Requests;
using static TaxService.Core.TaxServiceProcessor;

namespace TaxService.Data.TaxJar
{
    /// <summary>
    /// Implementation of IDataProvider to manage interactions with the TaxJar API (https://developers.taxjar.com/api/reference/#sales-tax-api)
    /// </summary>
    public class TaxJarProvider : IDataProvider
    {
        private const string CALCULATE_TAX_METHOD = "taxes";
        private const string GET_RATE_METHOD = "rates/{0}";
        private const string JSON_TYPE = "application/json";

        private readonly HttpClient _client;
        private readonly ILogger _logger;

        public TaxJarProvider(IOptions<ApiOptions> settings, ILogger<TaxJarProvider> logger)
        {
            _logger = logger;

            var url = settings.Value.Endpoint;
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogWarning("TaxJarDataProvider: API enpdoint is not configured");
                return;
            }

            if (!url.EndsWith("/"))
                url += "/";
            _client = new HttpClient { BaseAddress = new Uri(url) };

            if (!string.IsNullOrEmpty(settings.Value.Key))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.Value.Key);
        }

        public DataProviders ProviderType => DataProviders.TaxJar;

        public async Task<ITaxData> CalculateTax(CalculateTaxRequest request)
        {
            if (_client == null)
                return null;

            var tjRequest = new TaxRequest { 
                Amount = decimal.ToSingle(request.TaxableAmount), 
                DeliveryState = request.State, 
                DeliveryZipPostalCode = request.ZipPostalCode 
            };
            var requestData = JsonConvert.SerializeObject(tjRequest);

            var response = await _client.PostAsync(CALCULATE_TAX_METHOD, new StringContent(requestData, Encoding.UTF8, JSON_TYPE ));
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<TaxResponse>(responseData);
                
                return new TaxData { TotalTax = decimal.Parse(responseObject?.Tax?.TaxAmount.ToString()) };
            }

            throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");
        }

        public async Task<ITaxRateData> GetRate(GetRateRequest request)
        {
            if (_client == null)
                return null;

            var response = await _client.GetAsync(string.Format(GET_RATE_METHOD, request.ZipPostalCode));
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<RateResponse>(responseData);

                return new TaxRateData {
                    CityRate = decimal.Parse(responseObject?.Rate?.CityRate),
                    CountyRate = decimal.Parse(responseObject?.Rate?.CountyRate),
                    StateRate = decimal.Parse(responseObject?.Rate?.StateRate),
                    TotalRate = decimal.Parse(responseObject?.Rate?.CombinedRate) 
                };
            }

            throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");
        }
    }
}