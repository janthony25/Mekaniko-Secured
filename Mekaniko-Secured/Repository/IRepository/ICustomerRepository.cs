using Mekaniko_Secured.Models.Dto;

namespace Mekaniko_Secured.Repository.IRepository
{
    public interface ICustomerRepository
    {
        Task<List<CustomerSummaryDto>> GetCustomerListAsync();
        Task AddCustomerAsync(AddCustomerDto dto);
        Task<bool> DeleteCustomerByIdAsync(int id);
        Task<CustomerDto> GetCustomerCarsByIdAsync(int id);
        Task<bool> AddCarToCustomerAsync(AddCarDto dto);
    }
}
