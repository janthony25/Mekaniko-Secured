using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;

namespace Mekaniko_Secured.Controllers
{
    public class QuotationController : Controller
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly ILogger<QuotationController> _logger;
        public QuotationController(IQuotationRepository quotationRepository, ILogger<QuotationController> logger)
        {
            _quotationRepository = quotationRepository;
            _logger = logger;
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
    }
}
