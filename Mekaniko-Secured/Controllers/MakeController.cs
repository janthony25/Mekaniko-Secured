using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mekaniko_Secured.Controllers
{
    public class MakeController : Controller
    {
        private readonly IMakeRepository _makeRepository;

        public MakeController(IMakeRepository makeRepository)
        {
            _makeRepository = makeRepository;
        }
        // GET: Add Car Make
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AddNewMake()
        {
            var makes = await _makeRepository.GetAllMakesAsync();
            ViewBag.Makes = new SelectList(makes, "MakeId", "MakeName");

            return View(new AddMakeDto()); // Pass a new instance of AddMakeDto to the view
        }

        // POST: Add Car Make
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewMake(AddMakeDto makeDto)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    await _makeRepository.AddMakeAsync(makeDto);
                    TempData["SuccessMessage"] = "New make successfully added.";
                    return RedirectToAction("AddNewMake");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                
            }

            return View(makeDto);
        }



        // POST: Remove Car Make
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMake(AddMakeDto model)
        {
            if (model.MakeId != 0)
            {
                await _makeRepository.DeleteMakeAsync(model.MakeId);
                TempData["DeleteMessage"] = "Car make successfully deleted.";
                return RedirectToAction("AddNewMake");
            }

            // If MakeId is 0, reload makes and show the view again
            var makes = await _makeRepository.GetAllMakesAsync();
            ViewBag.Makes = new SelectList(makes, "MakeId", "MakeName");
            return View(model);
        }
    }
}
