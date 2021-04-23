using System.Threading.Tasks;
using TaxService.Library.Requests;
using TaxService.Library.Responses;

namespace TaxService.Core.Interfaces
{
    public interface ITaxServiceProcessor
    {
        Task<ProcessorResponse> CalculateTax(CalculateTaxRequest request);
        Task<ProcessorResponse> GetRate(GetRateRequest request);
    }
}