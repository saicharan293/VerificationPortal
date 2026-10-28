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
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)) 
                return RedirectToAction("Login", "Account");

            var courseLevel = HttpContext.Session.GetString("CourseLevel");

            if (string.IsNullOrWhiteSpace(courseLevel)) 
                return RedirectToAction("Login", "Account");

            // ========================================================= 
            // USER DETAILS 
            // =========================================================

            var user = await _context.TblRguhsFacultyUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new MyCollegesViewModel
            {
                UserName = user.UserName ?? string.Empty,
                UserDesignation = user.DesignationDescription ?? string.Empty,
                FacultyId = user.Faculty ?? 0

            };

            // Get user's college mappings (only active ones)
            var mappings = await _context.TblCollegeMappings
                .AsNoTracking()
                .Where(m => m.UserId == user.UserId && m.IsActive)
                .ToListAsync();


            if (mappings.Count == 0)
                return View(model);

            //GET FACULTY
            int facultyCodeInt = user.Faculty.Value;

            // Get faculty details
            var faculties = await GetFacultiesAsync(mappings);

            var eligibleCollegeCodes = await GetEligibleCollegeCodesAsync(courseLevel);

            // Fetch eligible colleges only ONCE
            var allEligibleColleges = await GetEligibleCollegesAsync(mappings, eligibleCollegeCodes);

            var academicYear = "2026-27";
            var applicationNumberByCollege = await GetApplicationNumbersAsync(allEligibleColleges, courseLevel, academicYear, facultyCodeInt.ToString());

            // Get eligible course codes for the selected course level
            var courses = await _context.MstCourses
                .AsNoTracking()
                .Where(c => c.CourseLevel == courseLevel)
                .Select(c => c.CourseCode)
                .ToListAsync();

            var courseCodes = courses.Select(c => c.ToString()) .ToHashSet();

            model.Mappings = await BuildCollegeMappingsAsync( mappings, allEligibleColleges, faculties, applicationNumberByCollege);
            return View(model);
        }

        private Task<List<CollegeMappingWithCollegesViewModel>> BuildCollegeMappingsAsync(List<TblCollegeMapping> mappings, List<AffiliationCollegeMaster> colleges, Dictionary<int, string> faculties, Dictionary<string, string> applicationNumbers)
        {
            var collegesByFaculty = colleges
                .GroupBy(c => c.FacultyCode)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = mappings.Select(mapping =>
            {
                var facultyCode = mapping.FacultyCode.ToString();

                var fromLetter = char.ToUpperInvariant(mapping.FromLetter?.FirstOrDefault() ?? 'A');

                var toLetter = char.ToUpperInvariant(mapping.ToLetter?.FirstOrDefault() ?? 'Z');

                var facultyColleges = collegesByFaculty.GetValueOrDefault(facultyCode) ?? new();

                var assignedColleges = facultyColleges
                    .Where(c =>
                        !string.IsNullOrWhiteSpace(c.CollegeName) &&
                        char.ToUpperInvariant(c.CollegeName[0]) >= fromLetter &&
                        char.ToUpperInvariant(c.CollegeName[0]) <= toLetter)
                    .Select(c => new CollegeDisplayViewModel
                    {
                        CollegeCode = c.CollegeCode,
                        CollegeName = c.CollegeName,
                        CollegeTown = c.CollegeTown,
                        ApplicationNumber = applicationNumbers.GetValueOrDefault(c.CollegeCode)
                    })
                    .ToList();

                return new CollegeMappingWithCollegesViewModel
                {
                    Mapping = mapping,
                    FacultyName = faculties.GetValueOrDefault(mapping.FacultyCode, "Unknown Faculty"),
                    Colleges = assignedColleges,
                    CollegeCount = assignedColleges.Count,
                    FromLetter = mapping.FromLetter,
                    ToLetter = mapping.ToLetter,
                    CollegeFromCode = mapping.CollegeFrom,
                    CollegeToCode = mapping.CollegeTo
                };
            }).ToList();

            return Task.FromResult(result);
        }


        private async Task<Dictionary<int, string>> GetFacultiesAsync( List<TblCollegeMapping> mappings)
        {
            var facultyIds = mappings
                .Select(m => m.FacultyCode)
                .Distinct()
                .ToList();

            return await _context.Faculties
                .AsNoTracking()
                .Where(f => facultyIds.Contains(f.FacultyId))
                .ToDictionaryAsync(f => f.FacultyId, f => f.FacultyName);
        }


        
        private async Task<HashSet<string>> GetEligibleCollegeCodesAsync(string courseLevel)
        {
            var courses = await _context.MstCourses
                .AsNoTracking()
                .Where(c => c.CourseLevel == courseLevel)
                .Select(c => c.CourseCode)
                .ToListAsync();

            var courseCodes = courses
                .Select(c => c.ToString())
                .ToHashSet();

            var academicIntakes = await _context.AcademicIntakes
                .AsNoTracking()
                .Where(a => a.Ay2026TotalIntake > 0)
                .Select(a => new
                {
                    a.CollegeCode,
                    a.Courses
                })
                .ToListAsync();

            return academicIntakes
                .Where(a =>
                    !string.IsNullOrWhiteSpace(a.Courses) &&
                    courseCodes.Contains(a.Courses))
                .Select(a => a.CollegeCode)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(code => code!)
                .ToHashSet();
        }


        private async Task<List<AffiliationCollegeMaster>> GetEligibleCollegesAsync( List<TblCollegeMapping> mappings, HashSet<string> eligibleCollegeCodes)
        {
            var facultyCodes = mappings
                .Select(m => m.FacultyCode.ToString())
                .Distinct()
                .ToList();

            return await _context.AffiliationCollegeMasters
                .AsNoTracking()
                .Where(c =>
                    facultyCodes.Contains(c.FacultyCode) &&
                    c.CollegeName != null &&
                    eligibleCollegeCodes.Contains(c.CollegeCode))
                .OrderBy(c => c.CollegeName)
                .ToListAsync();
        }

        private async Task<Dictionary<string, string>> GetApplicationNumbersAsync(List<AffiliationCollegeMaster> colleges, string courseLevel, string academicYear, string? facultyCode)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(facultyCode))
                return result;

            var collegeCodes = colleges
                .Where(c => c.FacultyCode == facultyCode)
                .Select(c => c.CollegeCode)
                .Distinct()
                .ToList();

            if (collegeCodes.Count == 0)
                return result;

            // Medical
            if (facultyCode == "1")
            {
                var applications = await _context.ApplicationSubmissions
                    .AsNoTracking()
                    .Where(e =>
                        e.FacultyCode == facultyCode &&
                        collegeCodes.Contains(e.CollegeCode) &&
                        e.CourseLevel == courseLevel)
                    .Select(e => new
                    {
                        e.CollegeCode,
                        e.RegistrationNumber
                    })
                    .ToListAsync();

                foreach (var application in applications
                            .GroupBy(e => e.CollegeCode)
                            .Select(g => g.First()))
                {
                    result[application.CollegeCode] = application.RegistrationNumber;
                }
            }
            // Dental
            else if (facultyCode == "2")
            {
                var applications = await _context.FinalDentalSubmissions
                    .AsNoTracking()
                    .Where(e =>
                        e.IsActive &&
                        e.AcademicYear == academicYear &&
                        collegeCodes.Contains(e.CollegeCode) &&
                        e.CourseLevel == courseLevel)
                    .Select(e => new
                    {
                        e.CollegeCode,
                        e.ApplicationNumber
                    })
                    .ToListAsync();

                foreach (var application in applications
                            .GroupBy(e => e.CollegeCode)
                            .Select(g => g.First()))
                {
                    result[application.CollegeCode] = application.ApplicationNumber;
                }
            }
            return result;
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