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

        private bool IsAdminUser()
        {
            var isAdminClaim = User.FindFirst("IsAdmin");
            return isAdminClaim != null && bool.TryParse(isAdminClaim.Value, out bool isAdmin) && isAdmin;
        }

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

            // ========================================================= 
            // USER DETAILS 
            // =========================================================

            var userDesignation = user.DesignationDescription ?? "";


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
                //colleges = colleges
                //    .Where(c =>
                //        string.Compare(c.CollegeCode, mapping.CollegeFrom, StringComparison.OrdinalIgnoreCase) >= 0
                //        && string.Compare(c.CollegeCode, mapping.CollegeTo, StringComparison.OrdinalIgnoreCase) <= 0)
                //    .ToList();

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

        [HttpGet]
        public async Task<IActionResult> GetVerificationStatuses( int facultyId, List<string> collegeCodes)
        {
            if (collegeCodes == null || !collegeCodes.Any())
            {
                return Json(new Dictionary<string, CollegeFeedbackStatusViewModel>());
            }

            var totalSections = await _context.MstSections
                .AsNoTracking()
                .CountAsync(s => s.FacultyId == facultyId);

            var allFeedback = await _context.SectionWiseFeedbacks
                .AsNoTracking()
                .Where(f =>
                    f.FacultyId == facultyId &&
                    collegeCodes.Contains(f.CollegeCode))
                .ToListAsync();

            var result =
                new Dictionary<string, CollegeFeedbackStatusViewModel>();

            foreach (var collegeCode in collegeCodes)
            {
                var collegeFeedback = allFeedback
                    .Where(f => f.CollegeCode == collegeCode)
                    .ToList();

                result[collegeCode] =
                    GetCollegeFeedbackStatus(
                        collegeFeedback,
                        totalSections);
            }

            return Json(result);
        }

        private CollegeFeedbackStatusViewModel GetCollegeFeedbackStatus( List<SectionWiseFeedback> collegeFeedback, int totalSections)
        {
            if (collegeFeedback == null || !collegeFeedback.Any())
            {
                return new CollegeFeedbackStatusViewModel
                {
                    Status = "Pending",
                    TotalSections = totalSections,
                    CompletedSections = 0,
                    PendingSections = totalSections,
                    RejectedSections = 0,
                    LastVerifiedOn = null,
                    LastVerifiedBy = null
                };
            }

            var verifiedSections = collegeFeedback
                .Where(f =>
                    !string.IsNullOrWhiteSpace(f.VerificationStatus) &&
                    (
                        f.VerificationStatus.Equals(
                            "Verified",
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        f.VerificationStatus.Equals(
                            "Approved",
                            StringComparison.OrdinalIgnoreCase)
                    ))
                .Select(f => f.SectionId)
                .Distinct()
                .Count();

            var rejectedSections = collegeFeedback
                .Where(f =>
                    !string.IsNullOrWhiteSpace(f.VerificationStatus) &&
                    f.VerificationStatus.Equals(
                        "Rejected",
                        StringComparison.OrdinalIgnoreCase))
                .Select(f => f.SectionId)
                .Distinct()
                .Count();

            var completedSectionIds = collegeFeedback
                .Where(f => !string.IsNullOrWhiteSpace(f.VerificationStatus))
                .Select(f => f.SectionId)
                .Distinct()
                .Count();

            var pendingSections = totalSections - completedSectionIds;

            if (pendingSections < 0)
            {
                pendingSections = 0;
            }

            var latestFeedback = collegeFeedback
                .Where(f => f.VerifiedOn.HasValue)
                .OrderByDescending(f => f.VerifiedOn)
                .FirstOrDefault();

            string overallStatus;

            // Priority 1:
            // Even ONE pending section means Pending
            if (pendingSections > 0)
            {
                overallStatus = "Pending";
            }

            // Priority 2:
            // No pending, but at least one rejected
            else if (rejectedSections > 0)
            {
                overallStatus = "Rejected";
            }

            // Priority 3:
            // No pending + no rejected = all accepted
            else if (verifiedSections == totalSections)
            {
                overallStatus = "Completed";
            }

            // Fallback
            else
            {
                overallStatus = "Pending";
            }

            return new CollegeFeedbackStatusViewModel
            {
                Status = overallStatus,
                TotalSections = totalSections,
                CompletedSections = completedSectionIds,
                PendingSections = pendingSections,
                RejectedSections = rejectedSections,
                LastVerifiedOn = latestFeedback?.VerifiedOn,
                LastVerifiedBy = latestFeedback?.VerifiedBy
            };
        }


    }
}