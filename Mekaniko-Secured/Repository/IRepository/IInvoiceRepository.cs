using Mekaniko_Secured.Models.Dto;

namespace Mekaniko_Secured.Repository.IRepository
{
    public interface IInvoiceRepository
    {
        Task AddInvoiceToCarAsync(AddCarInvoiceDto dto);
        Task<InvoiceDetailsDto> GetInvoiceDetailsAsync(int id);
        Task<List<InvoiceListDto>> GetInvoiceListAsync();
    }
}
