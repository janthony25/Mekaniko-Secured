using Mekaniko_Secured.Models;
using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Mekaniko_Secured.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigraDoc.Rendering;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Mekaniko_Secured.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoicePdfService _invoicePdfService;
        private readonly IEmailService _emailService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceRepository invoiceRepository, IInvoicePdfService invoicePdfService, IEmailService emailService, ILogger<InvoiceController> logger)
        {
            _invoiceRepository = invoiceRepository;
            _invoicePdfService = invoicePdfService;
            _emailService = emailService;
            _logger = logger;
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

        // GET: Generate PDF
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GeneratePdf(int id, bool download = false)
        {
            var invoice = await _invoiceRepository.GetInvoiceDetailsAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            var document = _invoicePdfService.CreateInvoicePdf(invoice);

            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();

            using (var stream = new MemoryStream())
            {
                renderer.PdfDocument.Save(stream, false);
                stream.Position = 0;
                byte[] byteArray = stream.ToArray();

                var contentDisposition = download ? "attachment" : "inline";
                Response.Headers.Add("Content-Disposition", $"{contentDisposition}; filename=Invoice_{id}.pdf");

                return File(byteArray, "application/pdf");
            }

        }

        // POST: SENDING PDF TO EMAIL
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendInvoiceEmail(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceDetailsAsync(invoiceId);
                if (invoice == null)
                {
                    return Json(new { success = false, message = "Invoice not found" });
                }


                // Generate the PDF using MigraDoc
                var document = _invoicePdfService.CreateInvoicePdf(invoice);
                var renderer = new PdfDocumentRenderer(true);
                renderer.Document = document;
                renderer.RenderDocument();

                // Convert the PDF to a byte array
                byte[] pdfBytes;
                using (MemoryStream stream = new MemoryStream())
                {
                    renderer.PdfDocument.Save(stream, false);
                    pdfBytes = stream.ToArray();
                }

                // Prepare email content
                string subject = $"Invoice #{invoice.InvoiceId} from Mobile Mekaniko";
                string body = $"Dear {invoice.CustomerName},\n\nPlease find your attached invoice (Invoice ID: {invoice.InvoiceId}).\n\nThank you for your business.\n\nBest regards,\nMobile Mekaniko";

                // Send the email
                await _emailService.SendEmailWithAttachmentAsync(
                    invoice.CustomerEmail,
                    subject,
                    body,
                    pdfBytes,
                    $"Invoice_{invoice.InvoiceId}.pdf"
                    );

                // after successfully sending email
                await _invoiceRepository.MarkEmailAsSentAsync(invoiceId);

                return Json(new { success = true, message = $"Email sent successfully to {invoice.CustomerName}" });
            }
            catch (Exception ex)
            {
                // Log the exceptoin
                return Json(new {success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        // POST: Mark Invoice as Paid
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(int invoiceId)
         {
            Console.WriteLine($"MarkAsPaid called with invoiceId: {invoiceId}");
            try
            {
                var result = await _invoiceRepository.MarkInvoiceAsPaidAsync(invoiceId);
                if (result)
                {
                    return Json(new { success = true, message = "Invoice marked as paid successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Invoice not found." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MarkAsPaid: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while marking the invoice as paid." });
            }
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoice(int invoiceId)
        {
            try
            {
                // Call the repository method to delete the invoice
                var result = await _invoiceRepository.DeleteInvoiceAsync(invoiceId);

                if (result)
                {
                    // If deletion was successful, return a success message
                    return Json(new { success = true, message = "Invoice deleted successfully." });
                }
                else
                {
                    // If the invoice wasn't found, return an error message
                    return Json(new { success = false, message = "Invoice not found." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error deleting invoice");

                // Return an error message
                return Json(new { success = false, message = "An error occurred while deleting the invoice." });
            }
        }

        // GET: Search Invoice by Rego
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> SearchInvoices(string rego)
        {
            var invoice = await _invoiceRepository.SearchInvoiceByRegoAsync(rego);
            return View("GetInvoiceSummary", invoice);
        }

        // GET: Filter by Status Paid
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> PaidInvoices(string status)
        {
            bool isPaid = status == "Paid";
            var invoiceStatus = await _invoiceRepository.FilterInvoicePaid(isPaid);
            return View("GetInvoiceSummary", invoiceStatus);
        }

        // GET: Filter by Unsent Email
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> FilterByEmailStatus(string status)
        {
            bool? isEmailSent = status switch
            {
                "sent" => true,
                "unsent" => false,
                "unknown" => null,
                _ => null
            };

            var invoices = await _invoiceRepository.FilterByEmailStatus(isEmailSent);
            return View("GetInvoiceSummary", invoices);
        }
    }
}
