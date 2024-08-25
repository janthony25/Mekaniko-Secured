using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mekaniko_Secured.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        // POST: Add Invoice to Car
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInvoiceToCar([FromBody] AddCarInvoiceDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return Json(new { success = false, message = "Invalid model state.", errors = errors });
            }

            try
            {
                await _invoiceRepository.AddInvoiceToCarAsync(dto);
                return Json(new { success = true, message = "Invoice added successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = "An error occurred while adding the invoice.", error = ex.Message });
            }
        }


        // GET: Invoice Details
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetInvoiceDetails(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceDetailsAsync(id);
                if (invoice == null)
                {
                    return Json(new { success = false, message = "Invoice not found." });
                }
                return Json(new { success = true, data = invoice });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while fetching invoice details." });
            }
        }


        // GET: Invoice List
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetInvoiceSummary()
        {
            var invoiceList = await _invoiceRepository.GetInvoiceListAsync();
            return View(invoiceList);
        }
    }
}
