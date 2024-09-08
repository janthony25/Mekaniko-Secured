using Mekaniko_Secured.Models.Dto;

namespace Mekaniko_Secured.Repository.IRepository
{
    public interface IMakeRepository
    {
        Task AddMakeAsync(AddMakeDto makeDto);
        Task DeleteMakeAsync(int id);
        Task<List<MakeListDto>> GetAllMakesAsync();
    }
}
