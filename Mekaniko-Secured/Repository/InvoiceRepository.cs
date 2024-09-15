using Mekaniko_Secured.Data;
using Mekaniko_Secured.Models;
using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<bool> DeleteInvoiceAsync(int invoiceId)
        {
            var invoice = await _data.Invoices.FindAsync(invoiceId);

            if (invoice == null)
            {
                return false;
            }

            _data.Invoices.Remove(invoice);
            await _data.SaveChangesAsync();

            return true;
        }

        public async Task<List<InvoiceListDto>> FilterByEmailStatus(bool? emailStatus)
        {
            return await _data.Invoices
                .Include(i => i.Car)
                    .ThenInclude(car => car.Customer)
                .Where(i => emailStatus.HasValue ?
                    (emailStatus.Value ? i.IsEmailSent == true : i.IsEmailSent == false) :
                    i.IsEmailSent == null)
                .OrderByDescending(i => i.DateAdded)
                .Select(i => new InvoiceListDto
                {
                    IsPaid = i.IsPaid,
                    IssueName = i.IssueName,
                    InvoiceId = i.InvoiceId,
                    CustomerName = i.Car.Customer.CustomerName,
                    DateAdded = i.DateAdded,
                    DueDate = i.DueDate,
                    CarRego = i.Car.CarRego,
                    TotalAmount = i.TotalAmount,
                    IsEmailSent = i.IsEmailSent
                }).ToListAsync();
        }
        public async Task<List<InvoiceListDto>> FilterInvoicePaid(bool status)
        {
            return await _data.Invoices
                .Include(i => i.Car)
                    .ThenInclude(car => car.Customer)
                .Where(i => i.IsPaid == status)  
                .Select(i => new InvoiceListDto
                {
                    IsPaid = i.IsPaid,
                    IssueName = i.IssueName,
                    InvoiceId = i.InvoiceId,
                    CustomerName = i.Car.Customer.CustomerName,
                    DateAdded = i.DateAdded,
                    DueDate = i.DueDate,
                    CarRego = i.Car.CarRego,
                    TotalAmount = i.TotalAmount,
                    IsEmailSent = i.IsEmailSent
                }).ToListAsync();
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
                .OrderByDescending(i => i.DateAdded)
                .Select(i => new InvoiceListDto
                {
                    IsPaid = i.IsPaid,
                    IsEmailSent = i.IsEmailSent,
                    IssueName = i.IssueName,
                    InvoiceId = i.InvoiceId,
                    DateAdded = i.DateAdded,
                    DueDate = i.DueDate,
                    CustomerName = i.Car.Customer.CustomerName,
                    CarRego = i.Car.CarRego,
                    TotalAmount = i.TotalAmount
                }).ToListAsync();
        }

        public async Task<decimal> GetRemainingBalanceAsync()
        {
            // Get the total amount of Invoice and paid amount
            var totalInvoiceAmount = await GetTotalInvoiceAmountAsync();
            var totalPaidAmount = await GetTotalPaidAmountAsync();

            // Calculate the remaining amount
            var remainingBalance = totalInvoiceAmount - totalPaidAmount;

            return remainingBalance;
        }

        public async Task<decimal> GetTotalInvoiceAmountAsync()
        {
            return await _data.Invoices.SumAsync(i => i.TotalAmount ?? 0m);
        }

        public async Task<int> GetTotalInvoiceCountAsync()
        {
            return await _data.Invoices.CountAsync();
        }

        public async Task<decimal> GetTotalPaidAmountAsync()
        {
            return await _data.Invoices.SumAsync(i => i.AmountPaid ?? 0m);
        }

        public async Task<List<InvoiceListDto>> GetUnpaidInvoicesAsync()
        {
            return await _data.Invoices
                .Include(i => i.Car)
                    .ThenInclude(car => car.Customer)
                .Where(i => i.IsPaid == false)
                .Select(i => new InvoiceListDto
                {
                    IsPaid = i.IsPaid,
                    IssueName = i.IssueName,
                    InvoiceId = i.InvoiceId,
                    CustomerName = i.Car.Customer.CustomerName,
                    DateAdded = i.DateAdded,
                    DueDate = i.DueDate,
                    CarRego = i.Car.CarRego,
                    TotalAmount = i.TotalAmount,
                    IsEmailSent = i.IsEmailSent
                }).ToListAsync();
        }

        public async Task<bool> MarkEmailAsSentAsync(int invoiceId)
        {
            var invoice = await _data.Invoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                invoice.IsEmailSent = true;
                await _data.SaveChangesAsync(); 
            }
            return false;
        }

        public async Task<bool> MarkInvoiceAsPaidAsync(int invoiceId)
        {
            var invoice = await _data.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                return false;
            }

            invoice.AmountPaid = invoice.TotalAmount;
            invoice.IsPaid = true;

            await _data.SaveChangesAsync();
            return true;
        }

        public async Task<List<InvoiceListDto>> SearchInvoiceByRegoAsync(string rego)
        {

            if(string.IsNullOrEmpty(rego))
            {
                return await GetInvoiceListAsync();
            }

            return await _data.Invoices
                .Include(i => i.Car)
                    .ThenInclude(car => car.Customer)
                .Where(i => i.Car.CarRego.Contains(rego))
                .Select(i => new InvoiceListDto
                {
                    IsPaid = i.IsPaid,
                    IssueName = i.IssueName,
                    InvoiceId = i.InvoiceId,
                    CustomerName = i.Car.Customer.CustomerName,
                    DateAdded = i.DateAdded,
                    DueDate = i.DueDate,
                    CarRego = i.Car.CarRego,
                    TotalAmount = i.TotalAmount,
                    IsEmailSent = i.IsEmailSent
                }).ToListAsync();
        }

        public async Task<bool> UpdateInvoiceNotesAsync(int invoiceId, string notes)
        {
            var invoice = await _data.Invoices.FindAsync(invoiceId);
            if (invoice == null)
            {
                return false;
            }

            invoice.Notes = notes;
            await _data.SaveChangesAsync();
            return true;
        }
    }
}
