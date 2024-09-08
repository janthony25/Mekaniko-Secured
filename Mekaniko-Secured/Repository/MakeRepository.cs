using Mekaniko_Secured.Data;
using Mekaniko_Secured.Models;
using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Mekaniko_Secured.Repository
{
    public class MakeRepository : IMakeRepository
    {
        private readonly DataContext _data;
        public MakeRepository(DataContext data)
        {
            _data = data;
        }
        public async Task AddMakeAsync(AddMakeDto makeDto)
        {
            // Check if the Make Name already exist
            var existingMake = await _data.Makes
                 .FirstOrDefaultAsync(m => m.MakeName == makeDto.MakeName);

            if (existingMake != null)
            {
                throw new InvalidOperationException("MakeName already exist");
            }

            // If MakeName does not exist
            var make = new Make
            {
                MakeName = makeDto.MakeName
            };

            _data.Add(make);
            await _data.SaveChangesAsync();

        }

        public async Task DeleteMakeAsync(int id)
        {
            // Fetch Make by Id
            var make = await _data.Makes.FindAsync(id);

            _data.Remove(make);
            await _data.SaveChangesAsync();
        }

        public async Task<List<MakeListDto>> GetAllMakesAsync()
        {
            return await _data.Makes
                .Select(m => new MakeListDto
                {
                    MakeId = m.MakeId,
                    MakeName = m.MakeName
                }).ToListAsync();
        }
    }
}
