using Mekaniko_Secured.Models.Dto;

namespace Mekaniko_Secured.Repository.IRepository
{
    public interface IInvoiceRepository
    {
        Task AddInvoiceToCarAsync(AddCarInvoiceDto dto);
        Task<InvoiceDetailsDto> GetInvoiceDetailsAsync(int id);
        Task<List<InvoiceListDto>> GetInvoiceListAsync();
        Task<int> GetTotalInvoiceCountAsync();
        Task<decimal> GetTotalInvoiceAmountAsync();
        Task<decimal> GetTotalPaidAmountAsync();
        Task<decimal> GetRemainingBalanceAsync();
        Task<List<UnpaidInvoiceListDto>> GetUnpaidInvoicesAsync();
        Task<bool> MarkInvoiceAsPaidAsync(int invoiceId);
        Task<bool> MarkEmailAsSentAsync(int invoiceId); 
    }
}
