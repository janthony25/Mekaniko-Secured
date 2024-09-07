using Mekaniko_Secured.Data;
using Mekaniko_Secured.Models;
using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Mekaniko_Secured.Repository
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly DataContext _data;
        private readonly ILogger<QuotationRepository> _logger;
        public QuotationRepository(DataContext data, ILogger<QuotationRepository> logger)
        {
            _data = data;
            _logger = logger;
        }

        public async Task AddCarQuotationAsync(AddCarQuotationDto dto)
        {
            try
            {
                // Fetch Car by Id
                var car = await _data.Cars.FindAsync(dto.CarId);
                if (car == null)
                {
                    throw new Exception($"Car with ID {dto.CarId} not found");
                }

                // Add New Quotation
                var quotation = new Quotation
                {
                    CarId = car.CarId,
                    DateAdded = dto.DateAdded,
                    IssueName = dto.IssueName,
                    Notes = dto.Notes,
                    LaborPrice = dto.LaborPrice,
                    Discount = dto.Discount,
                    ShippingFee = dto.ShippingFee,
                    SubTotal = dto.SubTotal,
                    TotalAmount = dto.TotalAmount
                };

                // Add Quotation to Db
                _data.Quotations.Add(quotation);
                await _data.SaveChangesAsync();

                // Add new Quotation Items
                var quotationItems = dto.QuotationItems.Select(item => new QuotationItem
                {
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    ItemPrice = item.ItemPrice,
                    ItemTotal = item.ItemTotal,
                    QuotationId = quotation.QuotationId
                }).ToList();

                // Add Quotation Items to Db
                _data.QuotationItems.AddRange(quotationItems);

                // Save changes async
                await _data.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the full exception details
                _logger.LogError(ex, "Error in AddCarQuotationAsync");
                throw; // Rethrow the exception to be caught in the controller
            }
        }

        public async Task<bool> DeleteQuotationAsync(int id)
        {
            // Fetch quotation id
            var quotation = await _data.Quotations.FindAsync(id);

            if (quotation != null)
            {
                _data.Quotations.Remove(quotation);
                await _data.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<CarQuotationSummaryDto>> GetCarQuotationSummaryAsync(int id)
        {
            return await _data.Cars
                .Include(car => car.Customer)
                .Include(car => car.CarMake)
                    .ThenInclude(cm => cm.Make)
                .Include(car => car.Quotation)
                .Where(car => car.CarId == id)
                .SelectMany(car => car.Quotation.DefaultIfEmpty(), (car, Quotation) => new CarQuotationSummaryDto
                {
                    CustomerId = car.Customer.CustomerId,
                    CustomerName = car.Customer.CustomerName,
                    CustomerEmail = car.Customer.CustomerEmail,
                    CustomerNumber = car.Customer.CustomerNumber,
                    CarId = car.CarId,
                    CarRego = car.CarRego,
                    MakeName = car.CarMake.Select(cm => cm.Make.MakeName).FirstOrDefault(),
                    CarModel = car.CarModel,
                    CarYear = car.CarYear,
                    QuotationId = Quotation != null ? Quotation.QuotationId : 0,
                    IssueName = Quotation.IssueName,
                    DateAdded = Quotation.DateAdded,
                    TotalAmount = Quotation.TotalAmount
                }).ToListAsync();
        }

        public async Task<QuotationDetailsDto> GetQuotationDetailsAsync(int id)
        {
            return await _data.Quotations
                .Include(q => q.Car)
                    .ThenInclude(car => car.CarMake)
                        .ThenInclude(cm => cm.Make)
                .Include(q => q.Car)
                    .ThenInclude(car => car.Customer)
                .Include(q => q.QuotationItem)
                .Where(q => q.QuotationId == id)
                .Select(q => new QuotationDetailsDto
                {
                    QuotationId = q.QuotationId,
                    CustomerName = q.Car.Customer.CustomerName,
                    CustomerEmail = q.Car.Customer.CustomerEmail,
                    CustomerNumber = q.Car.Customer.CustomerNumber,
                    CarId = q.Car.CarId,
                    CarRego = q.Car.CarRego,
                    MakeName = q.Car.CarMake.Select(cm => cm.Make.MakeName).FirstOrDefault(),
                    CarModel = q.Car.CarModel,
                    CarYear = q.Car.CarYear,
                    IssueName = q.IssueName,
                    Notes = q.Notes,
                    LaborPrice = q.LaborPrice,
                    Discount = q.Discount,
                    ShippingFee = q.ShippingFee,
                    SubTotal = q.SubTotal,
                    TotalAmount = q.TotalAmount,
                    QuotationItems = q.QuotationItem.Select(item => new QuotationItemDetailsDto
                    {
                        QuotationItemId = item.QuotationId,
                        ItemName = item.ItemName,
                        Quantity = item.Quantity,
                        ItemPrice = item.ItemPrice,
                        ItemTotal = item.ItemTotal
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<List<QuotationListDto>> GetQuotationListAsync()
        {
            return await _data.Quotations
                .Include(q => q.Car)
                    .ThenInclude(car => car.Customer)
                .Include(q => q.Car)
                    .ThenInclude(car => car.CarMake)
                        .ThenInclude(cm => cm.Make)
                .Select(q => new QuotationListDto
                {
                    QuotationId = q.QuotationId,
                    CustomerName = q.Car.Customer.CustomerName,
                    CarId = q.Car.CarId,
                    CarRego = q.Car.CarRego,
                    IssueName = q.IssueName,
                    DateAdded = q.DateAdded,
                    TotalAmount = q.TotalAmount,
                    IsEmailSent = q.IsEmailSent
                }).ToListAsync();
        }

        public async Task<int> GetTotalQuotationCountAsync()
        {
            return await _data.Quotations.CountAsync();
        }

        public async Task<bool> MarkEmailSentAsync(int id)
        {
            var quotation = await _data.Quotations.FindAsync(id);
            if (quotation != null)
            {
                quotation.IsEmailSent = true;
                await _data.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
