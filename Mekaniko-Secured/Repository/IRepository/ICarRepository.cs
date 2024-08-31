using Mekaniko_Secured.Models.Dto;

namespace Mekaniko_Secured.Repository.IRepository
{
    public interface ICarRepository
    {
        Task<bool> AddCarToCustomerAsync(AddCarDto dto);
        Task<List<MakeDto>> GetAllMakesAsync();
        Task<bool> DeleteCarByIdAsync(int id);
        Task<List<CarInvoiceSummaryDto>> GetCarInvoiceSummaryByCarIdAsync(int id);
        Task<int> GetTotalCarCountAsync();
    }
}
