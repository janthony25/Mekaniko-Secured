using Mekaniko_Secured.Data;
using Mekaniko_Secured.Models;
using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Mekaniko_Secured.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DataContext _data;

        public InvoiceRepository(DataContext data)
        {
            _data = data;
        }
        public async Task AddInvoiceToCarAsync(AddCarInvoiceDto dto)
        {
            // Fetching the CarId
            var car = await _data.Cars.FindAsync(dto.CarId);

            // Create new Invoice 
            var invoice = new Invoice
            {
                IssueName = dto.IssueName,
                DateAdded = dto.DateAdded,
                DueDate = dto.DueDate,
                PaymentTerm = dto.PaymentTerm,
                Notes = dto.Notes,
                LaborPrice = dto.LaborPrice,
                Discount = dto.Discount,
                ShippingFee = dto.ShippingFee,
                SubTotal = dto.SubTotal,
                //TaxAmount = dto.TaxAmount,
                TotalAmount = dto.TotalAmount,
                AmountPaid = dto.AmountPaid,
                IsPaid = dto.IsPaid,
                CarId = car.CarId,
            };

            // Add Invoice to Db
            _data.Invoices.Add(invoice);
            // Save Changes 
            await _data.SaveChangesAsync();

            // Add Invoice Item
            var invoiceItem = dto.InvoiceItems.Select(item => new InvoiceItem
            {
                ItemName = item.ItemName,
                Quantity = item.Quantity,
                ItemPrice = item.ItemPrice,
                ItemTotal = item.ItemTotal,
                InvoiceId = invoice.InvoiceId
            }).ToList();

            // Add InvoiceItem to Db
            _data.InvoiceItems.AddRange(invoiceItem);
            //Save Changes to Db
            await _data.SaveChangesAsync();
        }

        public async Task<InvoiceDetailsDto> GetInvoiceDetailsAsync(int id)
        {
            var invoice = await _data.Invoices
                 .Include(i => i.Car)
                     .ThenInclude(car => car.Customer)
                 .Include(i => i.InvoiceItem)
                 .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return null;
            }

            return new InvoiceDetailsDto
            {
                InvoiceId = invoice.InvoiceId,
                CustomerName = invoice.Car.Customer.CustomerName,
                CustomerEmail = invoice.Car.Customer.CustomerEmail,
                CustomerNumber = invoice.Car.Customer.CustomerNumber,
                CarId = invoice.CarId,
                CarRego = invoice.Car.CarRego,
                CarModel = invoice.Car.CarModel,
                CarYear = invoice.Car.CarYear,
                DateAdded = invoice.DateAdded,
                DueDate = invoice.DueDate,
                IssueName = invoice.IssueName,
                PaymentTerm = invoice.PaymentTerm,
                Notes = invoice.Notes,
                LaborPrice = invoice.LaborPrice,
                Discount = invoice.Discount,
                ShippingFee = invoice.ShippingFee,
                SubTotal = invoice.SubTotal,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = invoice.AmountPaid,
                IsPaid = invoice.IsPaid,
                InvoiceItems = invoice.InvoiceItem.Select(item => new InvoiceItemDto
                {
                    InvoiceItemId = item.InvoiceItemId,
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    ItemPrice = item.ItemPrice,
                    ItemTotal = item.ItemTotal
                }).ToList()
            };

        }

        public async Task<List<InvoiceListDto>> GetInvoiceListAsync()
        {
            return await _data.Invoices
                .Include(i => i.Car)
                    .ThenInclude(car => car.Customer)
                .Select(i => new InvoiceListDto
                {
                    InvoiceId = i.InvoiceId,
                    DateAdded = i.DateAdded,
                    DueDate = i.DueDate,
                    TotalAmount = i.TotalAmount,
                    IsPaid = i.IsPaid,
                    CustomerId = i.Car.Customer.CustomerId,
                    CustomerName = i.Car.Customer.CustomerName,
                    CarId = i.Car.CarId,
                    CarRego = i.Car.CarRego
                }).ToListAsync();
        }
    }
}
