using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;
namespace VerificationPortal.Controllers
{
    [Authorize]
    public class MyCollegesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MyCollegesController> _logger;
        private readonly ICollegeVerificationStatusService _verificationStatusService;

        public MyCollegesController(
            ApplicationDbContext context, 
            ICollegeVerificationStatusService verificationStatusService,
            ILogger<MyCollegesController> logger)
        {
            _context = context;
            _verificationStatusService = verificationStatusService;
            _logger = logger;

        }

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

        
        private Task<List<CollegeMappingWithCollegesViewModel>> BuildCollegeMappingsAsync( List<TblCollegeMapping> mappings, List<AffiliationCollegeMaster> colleges, Dictionary<int, string> faculties, Dictionary<string, string> applicationNumbers)
        {
            var result = mappings.Select(mapping =>
            {
                var facultyCode = mapping.FacultyCode.ToString();

                var range = mapping.AllCollegesMapping;

                var fromLetter = char.ToUpperInvariant(
                    range.FromLetter?.FirstOrDefault() ?? 'A');

                var toLetter = char.ToUpperInvariant(
                    range.ToLetter?.FirstOrDefault() ?? 'Z');


                var fromCollegeCode = range?.FromCollegeCode ?? "";
                var toCollegeCode = range?.ToCollegeCode ?? "";

                // Step 1: Get only assigned colleges
                var assignedColleges = colleges
                    .Where(c =>
                        c.FacultyCode == facultyCode &&
                        !string.IsNullOrWhiteSpace(c.CollegeCode) &&
                        string.Compare(
                            c.CollegeCode.Trim(),
                            fromCollegeCode.Trim(),
                            StringComparison.OrdinalIgnoreCase) >= 0 &&
                        string.Compare(
                            c.CollegeCode.Trim(),
                            toCollegeCode.Trim(),
                            StringComparison.OrdinalIgnoreCase) <= 0)
                    .ToList();

                // Step 2 & 3: Apply FromLetter and ToLetter filters
                var filteredColleges = assignedColleges
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
                    Colleges = filteredColleges,
                    CollegeCount = filteredColleges.Count,
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
            if (facultyId <= 0 || collegeCodes == null || !collegeCodes.Any())
            {
                return BadRequest(new { success = false, message = "Invalid request." });
            }

            // 1. Get Course Level from Session
            var courseLevel = HttpContext.Session.GetString("CourseLevel");

            if (string.IsNullOrWhiteSpace(courseLevel))
            {
                return Unauthorized();
            }

            // 2. Get Logged-in User ID
            if (!int.TryParse( User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                return Unauthorized();
            }

            // 3. Remove duplicates and invalid college codes
            collegeCodes = collegeCodes
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToList();

            if (!collegeCodes.Any())
            {
                return BadRequest(new { success = false, message = "No valid college codes." });
            }

            // 4. Get active mappings for logged-in user and faculty
            var mappings = await _context.TblCollegeMappings
                .AsNoTracking()
                .Where(m =>
                    m.UserId == userId &&
                    m.FacultyCode == facultyId &&
                    m.IsActive &&
                    m.AllCollegesMapping != null && 
                    m.AllCollegesMapping.FromCollegeCode != null &&
                    m.AllCollegesMapping.ToCollegeCode != null)
                .Select(m => new
                {
                    CollegeFrom = m.AllCollegesMapping!.FromCollegeCode,
                    CollegeTo = m.AllCollegesMapping!.ToCollegeCode
                })
                .ToListAsync();

            // 5. Verify that every requested college is authorized
            bool isAuthorized = collegeCodes.All(collegeCode =>
                mappings.Any(m =>
                    string.Compare(m.CollegeFrom, collegeCode, StringComparison.Ordinal) <= 0 &&
                    string.Compare(m.CollegeTo, collegeCode, StringComparison.Ordinal) >= 0
                ));

            if (!isAuthorized)
            {
                return Forbid();
            }

            // 6. Get Institution Types for all colleges
            var institutionTypes = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .Where(e => collegeCodes.Contains(e.CollegeCode))
                .Select(e => new
                {
                    e.CollegeCode,
                    e.TypeOfInstitution
                })
                .ToListAsync();

            var institutionTypeDictionary = institutionTypes
                .GroupBy(e => e.CollegeCode)
                .ToDictionary(
                    g => g.Key,
                    g => g.First().TypeOfInstitution
                );

            // 7. Get Overall Verification Status for each college
            var result = new Dictionary<string, object>();

            foreach (var collegeCode in collegeCodes)
            {
                institutionTypeDictionary.TryGetValue(collegeCode, out var typeOfInstitution);

                var status =
                    await _verificationStatusService.GetOverallVerificationStatusAsync( facultyId, collegeCode, courseLevel, typeOfInstitution );

                result[collegeCode] = new { status };
            }

            // 8. Return all college statuses
            return Json(new
            {
                success = true,
                statuses = result
            });
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

        //[HttpGet]
        //public async Task<IActionResult> GetCollegeVerificationStatus(string collegeCode, int facultyId)
        //{
        //    if(string.IsNullOrWhiteSpace(collegeCode)) return BadRequest(new { success = false });

        //    var courseLevel = HttpContext.Session.GetString("CourseLevel");

        //    if(string.IsNullOrWhiteSpace(courseLevel)) return Unauthorized();


        //    if(!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        //    {
        //        return Unauthorized();
        //    }

        //    var typeOfInstitution = await _context.AffInstitutionsDetails.Where(e => e.CollegeCode == collegeCode).Select(e => e.TypeOfInstitution).FirstOrDefaultAsync();
        //    var isAuthorized = await _context.TblCollegeMappings
        //        .AsNoTracking()
        //        .AnyAsync(m => 
        //            m.UserId == userId &&
        //            m.FacultyCode == facultyId &&
        //            m.IsActive &&
        //            m.CollegeFrom != null &&
        //            m.CollegeTo != null &&
        //            string.Compare(m.CollegeFrom, collegeCode) <= 0 &&
        //            string.Compare(m.CollegeTo, collegeCode) >= 0);
            
        //    if(!isAuthorized) return Forbid();

        //    var status = await _verificationStatusService.GetOverallVerificationStatusAsync(facultyId, collegeCode, courseLevel, typeOfInstitution);

        //    return Json(new
        //    {
        //        success = true,
        //        status
        //    });
        //}

    }
}