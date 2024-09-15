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
        Task<List<InvoiceListDto>> GetUnpaidInvoicesAsync();
        Task<bool> MarkInvoiceAsPaidAsync(int invoiceId);
        Task<bool> MarkEmailAsSentAsync(int invoiceId);
        Task<bool> DeleteInvoiceAsync(int invoiceId);
        Task<List<InvoiceListDto>> SearchInvoiceByRegoAsync(string rego);
        Task<List<InvoiceListDto>> FilterInvoicePaid(bool isPaid);
        Task<List<InvoiceListDto>> FilterByEmailStatus(bool? emailStatus);
        Task<bool> UpdateInvoiceNotesAsync(int invoiceId, string notes);
       // Task<List<InvoiceListDto>> UnpaidInvoices(bool isPaid);
    }
}
