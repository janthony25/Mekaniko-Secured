using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mekaniko_Secured.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICarRepository _carRepository;

        public DashboardController(IInvoiceRepository invoiceRepository, ICustomerRepository customerRepository, ICarRepository carRepository)
        {
            _invoiceRepository = invoiceRepository;
            _customerRepository = customerRepository;
            _carRepository = carRepository;
        }

        public ICustomerRepository CustomerRepository { get; }

        // GET: DASHBOARD HOME
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            // Get the total invoice amount, paid amount
            var totalInvoiceAmount = await _invoiceRepository.GetTotalInvoiceAmountAsync();
            var totalPaidAmount = await _invoiceRepository.GetTotalPaidAmountAsync();

            // Get the total number of customers, cars, invoices
            var totalCustomers = await _customerRepository.GetTotalCustomerCountAsync();
            var totalCars = await _carRepository.GetTotalCarCountAsync();
            var totalInvoices = await _invoiceRepository.GetTotalInvoiceCountAsync();
            var remainingBalance = await _invoiceRepository.GetRemainingBalanceAsync();

            // List of Unpaid Invoices
            var unpaidInvoices = await _invoiceRepository.GetUnpaidInvoicesAsync();

            // Create a view model to hold dashboard datas 
            var dashBoardDto = new DashboardDataDto
            {
                TotalInvoiceAmount = totalInvoiceAmount,
                TotalCustomers = totalCustomers,
                TotalCars = totalCars,
                TotalInvoices = totalInvoices,
                TotalPaidAmount = totalPaidAmount,
                RemainingBalance = remainingBalance,
                UnpaidInvoices = unpaidInvoices
            };

            return View(dashBoardDto);

        }

    }
}
