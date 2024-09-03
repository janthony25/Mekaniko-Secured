using Mekaniko_Secured.Models.Dto;

namespace Mekaniko_Secured.Repository.IRepository
{
    public interface IQuotationRepository
    {
        Task<List<CarQuotationSummaryDto>> GetCarQuotationSummaryAsync(int id);
        Task AddCarQuotationAsync(AddCarQuotationDto dto);
    }
}
