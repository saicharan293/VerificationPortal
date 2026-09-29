using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VerificationPortal.DATA;
using VerificationPortal.Models;

namespace VerificationPortal.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            // Get current user info from claims
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdClaim))
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            if (!int.TryParse( User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userDbId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.UserId == userDbId);

            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }

            // Get faculty details
            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(f => f.FacultyId == user.Faculty);

            // Get college mapping (if exists)
            var collegeMapping = await _context.TblCollegeMappings
                .FirstOrDefaultAsync(cm => cm.UserId == user.UserId && cm.IsActive == true);

            // Count statistics
            int totalColleges = 0;
            if (collegeMapping != null)
            {
                // This would be replaced with actual college table query
                // For demo, we'll use a placeholder
                totalColleges = 0;
            }

            var model = new DashboardViewModel
            {
                User = user,
                Faculty = faculty,
                CollegeMapping = collegeMapping,
                TotalAssignedColleges = totalColleges,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User",
                IsAdmin = user.IsAdmin ?? false,
                IsSection = user.IsSection ?? false,
                CurrentDate = DateTime.Now,
                Resources = GetResources()
            };

            //var affiliationTypeId = HttpContext.Session.GetInt32("AffiliationTypeId");

            //var courseLevel = HttpContext.Session.GetString("CourseLevel");

            //model.ShowAffiliationSelector = !affiliationTypeId.HasValue || string.IsNullOrEmpty(courseLevel);
            return View(model);
        }

        // GET: /Dashboard/Profile
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            int userDbId = int.Parse(userIdClaim);
            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.Id == userDbId);

            var faculty = await _context.Faculties
                .FirstOrDefaultAsync(f => f.FacultyId == user.Faculty);

            ViewBag.Faculty = faculty;
            return View(user);
        }

        private static List<DashboardResourceViewModel> GetResources()
        {
            return new List<DashboardResourceViewModel>
            {
                new()
                {
                    Title = "User Manual",
                    Description = "Complete guide for using the portal",
                    Icon = "bi bi-file-earmark-text",
                    Url = "#"
                },
                new()
                {
                    Title = "Guidelines 2024-25",
                    Description = "Latest affiliation guidelines",
                    Icon = "bi bi-file-earmark-pdf",
                    Url = "#"
                },
                new()
                {
                    Title = "Data Templates",
                    Description = "Download submission formats",
                    Icon = "bi bi-file-earmark-spreadsheet",
                    Url = "#"
                },
                new()
                {
                    Title = "FAQs",
                    Description = "Frequently asked questions",
                    Icon = "bi bi-question-circle",
                    Url = "#"
                },
                new()
                {
                    Title = "Contact Support",
                    Description = "Get help from our team",
                    Icon = "bi bi-headset",
                    Url = "#"
                },
                new()
                {
                    Title = "Announcements",
                    Description = "Latest updates & circulars",
                    Icon = "bi bi-megaphone",
                    Url = "#"
                }
            };
        }
    }
}
