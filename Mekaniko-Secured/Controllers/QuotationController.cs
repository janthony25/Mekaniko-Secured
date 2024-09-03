using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Mekaniko_Secured.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigraDoc.Rendering;
using System.Reflection.Emit;

namespace Mekaniko_Secured.Controllers
{
    public class QuotationController : Controller
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly ILogger<QuotationController> _logger;
        private readonly IQuotationPdfService _quotationPdfService;
        public QuotationController(IQuotationRepository quotationRepository, ILogger<QuotationController> logger, IQuotationPdfService quotationPdfService)
        {
            _quotationRepository = quotationRepository;
            _logger = logger;
            _quotationPdfService = quotationPdfService;
        }

        // GET: Quotation summary
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCarQuotation(int id)
        {
            var carQuotation = await _quotationRepository.GetCarQuotationSummaryAsync(id);
            return View(carQuotation);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuotationToCar([FromBody] AddCarQuotationDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return Json(new { success = false, message = "Invalid model state", errors = errors });
            }
            try
            {
                await _quotationRepository.AddCarQuotationAsync(dto);
                return Json(new { success = true, message = "Quotation added successfully." });
            }
            catch (Exception ex)
            {
                // Log the full exception details
                _logger.LogError(ex, "Error occurred while adding quotation");
                return Json(new { success = false, message = $"An error occurred while adding quotation: {ex.Message}" });
            }
        }

        // GENERATE PDF
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GeneratePdf(int id, bool download = false)
        {
            var quotation = await _quotationRepository.GetQuotationDetailsAsync(id);
            if (quotation == null)
            {
                return NotFound();
            }

            var document = _quotationPdfService.CreateQuotationPdf(quotation);

            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();

            using (var stream = new MemoryStream())
            {
                renderer.PdfDocument.Save(stream, false);
                stream.Position = 0;
                byte[] pdfBytes = stream.ToArray();

                var contentDisposition = download ? "attachment" : "inline";
                Response.Headers.Add("Content-Disposition", $"{contentDisposition}; filename=Quotation_{id}.pdf");

                return File(pdfBytes, "application/pdf");
            }
        }

    }
}
