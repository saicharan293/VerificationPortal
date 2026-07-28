using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VerificationPortal.DATA;
using VerificationPortal.Models;
namespace VerificationPortal.Controllers
{
    [Authorize]
    public class MyCollegesController(ApplicationDbContext context, ILogger<MyCollegesController> logger) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<MyCollegesController> _logger = logger;

        // GET: /MyColleges
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            int userDbId = int.Parse(userIdClaim);
            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.Id == userDbId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user's college mappings (only active ones)
            var mappings = await _context.TblCollegeMappings
                .Where(m => m.UserId == user.UserId && m.IsActive)
                .Include(m => m.FacultyCodeNavigation)
                .ToListAsync();

            if (mappings.Count == 0)
            {
                ViewBag.Message = "You have no college mappings assigned.";
                ViewBag.UserName = user.UserName;
                ViewBag.UserDesignation = user.DesignationDescription ?? "";
                ViewBag.FacultyId = user.Faculty ?? 0;
                return View(new List<CollegeMappingWithCollegesViewModel>());
            }

            // Get faculty details
            var facultyIds = mappings.Select(m => m.FacultyCode).Distinct().ToList();
            var faculties = await _context.Faculties
                .Where(f => facultyIds.Contains(f.FacultyId))
                .ToDictionaryAsync(f => f.FacultyId, f => f.FacultyName);

            // Get user designation
            var userDesignation = user.DesignationDescription ?? "";

            var viewModels = new List<CollegeMappingWithCollegesViewModel>();

            foreach (var mapping in mappings)
            {
                // Get all colleges in the mapped range for this faculty
                var colleges = await _context.AffiliationCollegeMasters
                    .Where(c => c.FacultyCode == mapping.FacultyCode.ToString()
                             && c.CollegeName != null)
                    .OrderBy(c => c.CollegeName)
                    .ToListAsync();

                // Filter by the alphabetical range (FromLetter to ToLetter)
                colleges = colleges
                    .Where(c =>
                    {
                        var letter = c.CollegeName![0].ToString().ToUpper();
                        return string.Compare(letter, mapping.FromLetter, StringComparison.OrdinalIgnoreCase) >= 0
                            && string.Compare(letter, mapping.ToLetter, StringComparison.OrdinalIgnoreCase) <= 0;
                    })
                    .ToList();

                // Further filter by college code range (CollegeFrom to CollegeTo)
                colleges = colleges
                    .Where(c =>
                        string.Compare(c.CollegeCode, mapping.CollegeFrom, StringComparison.OrdinalIgnoreCase) >= 0
                        && string.Compare(c.CollegeCode, mapping.CollegeTo, StringComparison.OrdinalIgnoreCase) <= 0)
                    .ToList();

                var facultyName = faculties.GetValueOrDefault(mapping.FacultyCode, "Unknown Faculty");

                viewModels.Add(new CollegeMappingWithCollegesViewModel
                {
                    Mapping = mapping,
                    FacultyName = facultyName,
                    UserDesignation = userDesignation,
                    Colleges = colleges,
                    CollegeCount = colleges.Count,
                    FromLetter = mapping.FromLetter,
                    ToLetter = mapping.ToLetter,
                    CollegeFromCode = mapping.CollegeFrom,
                    CollegeToCode = mapping.CollegeTo
                });
            }

            ViewBag.UserName = user.UserName;
            ViewBag.UserDesignation = userDesignation;
            ViewBag.FacultyId = user.Faculty ?? 0;

            return View(viewModels);
        }
    }
}