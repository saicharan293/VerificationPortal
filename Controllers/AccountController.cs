using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VerificationPortal.DATA;
using VerificationPortal.Models;

namespace VerificationPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already logged in, redirect to dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Find user by username
                var user = await _context.TblRguhsFacultyUsers
                    .FirstOrDefaultAsync(u => u.UserName == model.UserName);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }

                // Check if account is locked
                if (user.LockoutEndTime.HasValue && user.LockoutEndTime > DateTime.Now)
                {
                    ModelState.AddModelError("", $"Account is locked until {user.LockoutEndTime.Value:dd MMM yyyy HH:mm}");
                    return View(model);
                }

                // Check if account is active
                if (!user.IsActive)
                {
                    ModelState.AddModelError("", "Your account is inactive. Please contact administrator.");
                    return View(model);
                }

                // Verify password (check both Password and PasswordHash)
                bool isValidPassword = false;

                // Check plain text password (for development)
                if (!string.IsNullOrEmpty(user.Password) && user.Password == model.Password)
                {
                    isValidPassword = true;
                }
                // Check hashed password
                else if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    isValidPassword = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                }

                if (!isValidPassword)
                {
                    // Increment failed login attempts
                    user.FailedLoginAttempts += 1;

                    // Lock account after 5 failed attempts
                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.LockoutEndTime = DateTime.Now.AddMinutes(30);
                    }

                    await _context.SaveChangesAsync();

                    int remainingAttempts = 5 - user.FailedLoginAttempts;
                    if (remainingAttempts > 0)
                    {
                        ModelState.AddModelError("", $"Invalid password. {remainingAttempts} attempt(s) remaining.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Account locked due to multiple failed attempts. Try again after 30 minutes.");
                    }
                    return View(model);
                }

                // Reset failed attempts on successful login
                user.FailedLoginAttempts = 0;
                user.LockoutEndTime = null;
                await _context.SaveChangesAsync();

                // Get faculty name
                var faculty = await _context.Faculties
                    .FirstOrDefaultAsync(f => f.FacultyId == user.Faculty);

                // Determine role
                string userRole = DetermineUserRole(user);

                // Create claims for authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("FullName", user.UserName),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim("FacultyId", user.Faculty?.ToString() ?? ""),
                    new Claim("FacultyCode", faculty?.FacultyId.ToString()),
                    new Claim("FacultyName", faculty?.FacultyName ?? ""),
                    new Claim("Designation", user.DesignationDescription ?? ""),
                    new Claim(ClaimTypes.Role, userRole),
                    new Claim("IsAdmin", (user.IsAdmin ?? false).ToString()),
                    new Claim("IsSection", (user.IsSection ?? false).ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation($"User {user.UserName} logged in successfully with role {userRole}");

                // Redirect based on role or return URL
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                TempData["SuccessMessage"] = $"Welcome back, {user.DesignationDescription}!";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userName = User.Identity?.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation($"User {userName} logged out.");
            TempData["InfoMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Helper method to determine user role
        private string DetermineUserRole(TblRguhsFacultyUser user)
        {
            if (user.IsAdmin == true) return "Admin";
            if (user.IsSection == true) return "SectionOfficer";

            // Designation-based roles
            if (!string.IsNullOrEmpty(user.DesignationDescription))
            {
                var desc = user.DesignationDescription.ToLower();
                if (desc.Contains("vice chancellor")) return "ViceChancellor";
                if (desc.Contains("director")) return "Director";
                if (desc.Contains("registrar evaluation")) return "RegistrarEvaluation";
                if (desc.Contains("registrar")) return "Registrar";
                if (desc.Contains("assistant registrar")) return "AssistantRegistrar";
                if (desc.Contains("section officer")) return "SectionOfficer";
                if (desc.Contains("junior")) return "JuniorAssistant";
                if (desc.Contains("data entry")) return "DataEntryOperator";
            }

            return "User";
        }
    }
}
