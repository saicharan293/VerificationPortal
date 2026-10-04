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
            if (!int.TryParse(userIdClaim, out var userId)) return RedirectToAction("Login", "Account");

            var courseLevel = HttpContext.Session.GetString("CourseLevel");

            if (string.IsNullOrWhiteSpace(courseLevel)) return RedirectToAction("Login", "Account");

            // ========================================================= 
            // USER DETAILS 
            // =========================================================

            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user's college mappings (only active ones)
            var mappings = await _context.TblCollegeMappings
                .Where(m => m.UserId == user.UserId && m.IsActive)
                .ToListAsync();

            var model = new MyCollegesViewModel
            {
                UserName = user.UserName ?? string.Empty,
                UserDesignation = user.DesignationDescription ?? string.Empty,
                FacultyId = user.Faculty ?? 0

            };

            if (mappings.Count == 0)
            {
                return View(model);
            }


            // Get faculty details
            var facultyIds = mappings
                .Select(m => m.FacultyCode)
                .Distinct()
                .ToList();

            var faculties = await _context.Faculties
                .AsNoTracking()
                .Where(f => facultyIds.Contains(f.FacultyId))
                .ToDictionaryAsync(f => f.FacultyId, f => f.FacultyName);

            // Get eligible course codes for the selected course level
            var courses = await _context.MstCourses
                .AsNoTracking()
                .Where(c => c.CourseLevel == courseLevel)
                .Select(c => c.CourseCode)
                .ToListAsync();

            var courseCodes = courses
                .Select(c => c.ToString())
                .ToHashSet();

            // Get college codes with a positive 2026 intake
            var academicIntakes = await _context.AcademicIntakes
                .AsNoTracking()
                .Where(a => a.Ay2026TotalIntake > 0)
                .Select(a => new
                {
                    a.CollegeCode,
                    a.Courses
                })
                .ToListAsync();

            var eligibleCollegeCodes = academicIntakes
                .Where(a =>
                    !string.IsNullOrWhiteSpace(a.Courses) &&
                    courseCodes.Contains(a.Courses.ToString()))
                .Select(a => a.CollegeCode)
                .Where(code => code != null)
                .Distinct()
                .ToHashSet();

            // Prepare faculty codes for the database query
            var facultyCodeStrings = mappings
                .Select(m => m.FacultyCode.ToString())
                .Distinct()
                .ToList();

            // Fetch eligible colleges only ONCE
            var allEligibleColleges = await _context.AffiliationCollegeMasters
                .AsNoTracking()
                .Where(c =>
                    facultyCodeStrings.Contains(c.FacultyCode) &&
                    c.CollegeName != null &&
                    eligibleCollegeCodes.Contains(c.CollegeCode))
                .OrderBy(c => c.CollegeName)
                .ToListAsync();

            // Dictionary for quick faculty-based lookup
            var collegesByFaculty = allEligibleColleges
                .GroupBy(c => c.FacultyCode)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());

            // Process each mapping without additional database queries
            foreach (var mapping in mappings)
            {
                var facultyCode = mapping.FacultyCode.ToString();

                var fromLetter =
                    char.ToUpperInvariant(
                        mapping.FromLetter?.FirstOrDefault() ?? 'A');

                var toLetter =
                    char.ToUpperInvariant(
                        mapping.ToLetter?.FirstOrDefault() ?? 'Z');

                // Dictionary lookup instead of fetching from DB again
                var facultyColleges = collegesByFaculty
                    .GetValueOrDefault(facultyCode) ?? new();

                var colleges = facultyColleges
                    .Where(c =>
                    {
                        var firstLetter =
                            char.ToUpperInvariant(c.CollegeName![0]);

                        return firstLetter >= fromLetter &&
                               firstLetter <= toLetter;
                    })
                    .ToList();

                model.Mappings.Add(
                    new CollegeMappingWithCollegesViewModel
                    {
                        Mapping = mapping,

                        FacultyName = faculties.GetValueOrDefault(
                            mapping.FacultyCode,
                            "Unknown Faculty"),

                        Colleges = colleges,
                        CollegeCount = colleges.Count,

                        FromLetter = mapping.FromLetter,
                        ToLetter = mapping.ToLetter,

                        CollegeFromCode = mapping.CollegeFrom,
                        CollegeToCode = mapping.CollegeTo
                    });
            }

            return View(model);
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