using Mekaniko_Secured.Data;
using Mekaniko_Secured.Models;
using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Mekaniko_Secured.Repository
{
    public class CarRepository : ICarRepository
    {
        private readonly DataContext _data;
        public CarRepository(DataContext data)
        {
            _data = data;
        }
        public async Task<bool> AddCarToCustomerAsync(AddCarDto dto)
        {
            // Get the customer
            var customer = await _data.Customers.FindAsync(dto.CustomerId);

            // Create a new car
            var car = new Car
            {
                CarRego = dto.CarRego,
                CarModel = dto.CarModel,
                CarYear = dto.CarYear,
                CustomerId = customer.CustomerId
            };

            // Add Car to Db
            _data.Cars.Add(car);

            // Save Changes to Db
            await _data.SaveChangesAsync();

            // Find Make
            var make = await _data.Makes.FindAsync(dto.MakeId);

            // Create New CarMake   
            var carMake = new CarMake
            {
                CarId = car.CarId,
                MakeId = make.MakeId
            };

            // Add CarMake to Db
            _data.CarMakes.Add(carMake);

            // Save Changes to Db
            await _data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCarByIdAsync(int id)
        {
            // Find Car By Id
            var car = await _data.Cars.FindAsync(id);
            if (car != null)
            {
                _data.Cars.Remove(car);
                await _data.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<MakeDto>> GetAllMakesAsync()
        {
            var makes = await _data.Makes.Select(m => new MakeDto
            {
                MakeId = m.MakeId,
                MakeName = m.MakeName
            }).ToListAsync();

            return makes;
        }

        public async Task<List<CarInvoiceSummaryDto>> GetCarInvoiceSummaryByCarIdAsync(int id)
        {
            return await _data.Cars
                .Include(car => car.Customer)
                .Include(car => car.CarMake)
                    .ThenInclude(cm => cm.Make)
                .Include(car => car.Invoice)
                .Where(car => car.CarId == id)
                .SelectMany(car => car.Invoice.DefaultIfEmpty(), (car, invoice) => new CarInvoiceSummaryDto
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
                    CarPaymentStatus = car.CarPaymentStatus,
                    InvoiceId = invoice != null ? invoice.InvoiceId : 0,
                    IssueName = invoice.IssueName,
                    DateAdded = invoice.DateAdded,
                    DueDate = invoice.DueDate,
                    TotalAmount = invoice.TotalAmount,
                    AmountPaid = invoice.AmountPaid,
                    IsPaid = invoice.IsPaid
                }).ToListAsync();
        }
    }
}
