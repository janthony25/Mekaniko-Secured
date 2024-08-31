using Mekaniko_Secured.Data;
using Mekaniko_Secured.Models;
using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Mekaniko_Secured.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _data;
        public CustomerRepository(DataContext data)
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

        public async Task AddCustomerAsync(AddCustomerDto dto)
        {
            var customer = new Customer
            {
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerNumber = dto.CustomerNumber
            };

            // Add customer to DB
            _data.Customers.Add(customer);
            // Save changes to DB
            await _data.SaveChangesAsync();
        }

        public async Task<bool> DeleteCustomerByIdAsync(int id)
        {
            // Find Customer
            var customer = await _data.Customers.FindAsync(id);

            if (customer != null)
            {
                _data.Customers.Remove(customer);
                await _data.SaveChangesAsync();
                return true; // Deletion was successful
            }
            return false; // Customer not found
        }

        public async Task<CustomerDto> GetCustomerCarsByIdAsync(int id)
        {
            return await _data.Customers
                .Include(c => c.Car)
                    .ThenInclude(car => car.CarMake)
                        .ThenInclude(cm => cm.Make)
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    CustomerEmail = c.CustomerEmail,
                    CustomerNumber = c.CustomerNumber,
                    Car = c.Car.Select(car => new CarDto
                    {
                        CarId = car.CarId,
                        CarRego = car.CarRego,
                        CarModel = car.CarModel,
                        CarYear = car.CarYear,
                        CarPaymentStatus = car.CarPaymentStatus,
                        Make = car.CarMake.Select(cm => new MakeDto
                        {
                            MakeName = cm.Make.MakeName
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<List<CustomerSummaryDto>> GetCustomerListAsync()
        {
            return await _data.Customers
                .Select(c => new CustomerSummaryDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    CustomerEmail = c.CustomerEmail,
                    CustomerNumber = c.CustomerNumber
                }).ToListAsync();
        }

        public async Task<int> GetTotalCustomerCountAsync()
        {
            return await _data.Customers.CountAsync();
        }
    }
}
