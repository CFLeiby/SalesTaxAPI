using System.Threading.Tasks;
using TaxService.Library.Requests;

namespace TaxService.Core.Interfaces
{
    public interface IDataProvider
    {
        Task<ITaxData> CalculateTax(CalculateTaxRequest request);
        Task<ITaxRateData> GetRate(GetRateRequest request);
        TaxServiceProcessor.DataProviders ProviderType { get; }
    }
}