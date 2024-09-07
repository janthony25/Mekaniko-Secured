using Mekaniko_Secured.Models.Dto;

namespace Mekaniko_Secured.Repository.IRepository
{
    public interface IQuotationRepository
    {
        Task<List<CarQuotationSummaryDto>> GetCarQuotationSummaryAsync(int id);
        Task AddCarQuotationAsync(AddCarQuotationDto dto);
        Task<QuotationDetailsDto> GetQuotationDetailsAsync(int id);
        Task<bool> MarkEmailSentAsync(int id);
        Task<int> GetTotalQuotationCountAsync();
        Task<List<QuotationListDto>> GetQuotationListAsync();
        Task<bool> DeleteQuotationAsync(int id);
    }
}
