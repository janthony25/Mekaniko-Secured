using Mekaniko_Secured.Models.Dto;
using Mekaniko_Secured.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

namespace Mekaniko_Secured.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _userService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(UserService userService, ILogger<LoginController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation($"Accessing Login/Index. User authenticated: {User.Identity.IsAuthenticated}");
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginDto loginDto)
        {
            _logger.LogInformation($"Login attempt for user: {loginDto.Username}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                return View(loginDto);
            }

            var (success, message) = await _userService.AuthenticateUser(loginDto);

            if (success)
            {
                _logger.LogInformation($"User {loginDto.Username} authenticated successfully");
                var authenticatedUser = await _userService.GetUserByUsername(loginDto.Username);
                if (authenticatedUser != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, authenticatedUser.Username),
                        new Claim(ClaimTypes.Role, authenticatedUser.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation($"Authentication cookie set for user: {authenticatedUser.Username} with role: {authenticatedUser.Role}");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogError($"User {loginDto.Username} not found after successful authentication");
                    ModelState.AddModelError(string.Empty, "User not found after authentication");
                }
            }
            else
            {
                _logger.LogWarning($"Authentication failed for user {loginDto.Username}: {message}");
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }

            return View(loginDto);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Login");
        }


        // GET: Change Password
        [Authorize(Policy = "Admin")]
        public IActionResult ChangePassword()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var model = new ChangePasswordDto
            {
                Username = username
            };
            return View(model);
        }

        // POST: Change Password
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordDto);
            }

            var (success, message) = await _userService.ChangePassword(changePasswordDto);

            if (success)
            {
                _logger.LogInformation($"Password changed successfully for user: {changePasswordDto.Username}");
                TempData["SuccessMessage"] = "Password changed successfully";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _logger.LogWarning($"Password change fail for user {changePasswordDto.Username}: {message}");
                ModelState.AddModelError(string.Empty, message);
                return View(changePasswordDto);
            }
        }

    }
}
