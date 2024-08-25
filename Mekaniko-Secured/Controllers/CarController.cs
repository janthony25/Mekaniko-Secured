using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mekaniko_Secured.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly ICustomerRepository _customerRepository;

        public CarController(ICarRepository carRepository, ICustomerRepository customerRepository)
        {
            _carRepository = carRepository;
            _customerRepository = customerRepository;
        }



        // POST: Car/AddCar
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCarToCustomer([FromBody] AddCarDto dto)
        {
            if (ModelState.IsValid)
            {
                // Your existing logic to add the car
                var result = await _customerRepository.AddCarToCustomerAsync(dto);
                if (result)
                {
                    return Json(new { success = true, message = "Car added successfully." });
                }
            }

            // If we get here, something went wrong
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return Json(new { success = false, errors = errors });
        }

        // POST: Delete Car
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCar(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid car ID." });
            }

            try
            {
                var result = await _carRepository.DeleteCarByIdAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "Car deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Car not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error occurred while deleting car: {ex.Message}" });
            }
        }

        // GET: Car Invoice
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCarInvoice(int id)
        {
            var carInvoice = await _carRepository.GetCarInvoiceSummaryByCarIdAsync(id);
            return View(carInvoice);
        }
    }
}
