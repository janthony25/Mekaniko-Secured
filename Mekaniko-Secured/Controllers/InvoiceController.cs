﻿using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Mekaniko_Secured.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigraDoc.Rendering;

namespace Mekaniko_Secured.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoicePdfService _invoicePdfService;

        public InvoiceController(IInvoiceRepository invoiceRepository, IInvoicePdfService invoicePdfService)
        {
            _invoiceRepository = invoiceRepository;
            _invoicePdfService = invoicePdfService;
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
    }
}
