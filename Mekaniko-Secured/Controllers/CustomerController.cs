using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mekaniko_Secured.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICarRepository _carRepository;
        public CustomerController(ICustomerRepository customerRepository, ICarRepository carRepository)
        {
            _customerRepository = customerRepository;
            _carRepository = carRepository;
        }

        // GET - Customer List
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCustomerList()
        {
            var customer = await _customerRepository.GetCustomerListAsync();
            return View(customer);
        }

        // POST - Add New Customer
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewCustomer([FromBody] AddCustomerDto dto)
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.AddCustomerAsync(dto);
                return Json(new { success = true, message = "Customer Added Successfully!" });
            }

            return Json(new { success = false, message = "Failed to add customer." });
        }

        // POST - Delete Customer
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid customer ID." });
            }

            try
            {
                var result = await _customerRepository.DeleteCustomerByIdAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "Customer deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Customer not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error occurred while deleting customer: {ex.Message}" });
            }
        }

        // GET - Customer Cars
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetCustomerCars(int id)
        {
            var customerCar = await _customerRepository.GetCustomerCarsByIdAsync(id);
            var makes = await _carRepository.GetAllMakesAsync();
            ViewBag.Makes = makes;
            return View(customerCar);
        }


        // GET: Search Customer
        [Authorize (Policy = "Admin")]
        public async Task<IActionResult> SearchCustomer(string customerName)
        {
            var customer = await _customerRepository.SearchCustomerByNameAsync(customerName);
            return View("GetCustomerList", customer);
        }
    }
}
