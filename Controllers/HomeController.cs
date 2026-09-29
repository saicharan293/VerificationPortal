using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using VerificationPortal.Models;
using VerificationPortal.Models.ViewModels;

namespace VerificationPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                TotalColleges = 1247,
                VerifiedColleges = 1180,
                PendingColleges = 67,
                TotalFaculties = 8,
                TotalDistricts = 31,
                RecentVerifications = new List<RecentActivity>
                {
                    new() { CollegeName = "ABC Medical College, Bangalore", Faculty = "Medical", Status = "Verified", Date = DateTime.Now.AddDays(-1), VerifiedBy = "AR" },
                    new() { CollegeName = "XYZ Dental College, Mysore", Faculty = "Dental", Status = "Pending", Date = DateTime.Now.AddDays(-2), VerifiedBy = "DEO" },
                    new() { CollegeName = "PQR Pharmacy Institute, Hubli", Faculty = "Pharmacy", Status = "Verified", Date = DateTime.Now.AddDays(-3), VerifiedBy = "SO" },
                },
                Notifications = new List<Notification>
                {
                    new() { Title = "New circular on faculty verification", Date = DateTime.Now.AddDays(-1), Type = "Important" },
                    new() { Title = "Updated guidelines for 2024-25 affiliation", Date = DateTime.Now.AddDays(-3), Type = "Update" },
                    new() { Title = "Training workshop for DEOs", Date = DateTime.Now.AddDays(-5), Type = "Event" },
                }
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public IActionResult SetContext( [FromBody] AffiliationContextRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            if (request.AffiliationTypeId <= 0)
            {
                return BadRequest("Invalid affiliation type.");
            }

            if (string.IsNullOrWhiteSpace(request.CourseLevel))
            {
                return BadRequest("Course level is required.");
            }


            // Store context in session

            HttpContext.Session.SetInt32("AffiliationTypeId", request.AffiliationTypeId);

            HttpContext.Session.SetString("CourseLevel", request.CourseLevel);


            return Ok(new
            {
                success = true
            });
        }

        public IActionResult Privacy() => View();
        public IActionResult Error() => View();
    }
}
