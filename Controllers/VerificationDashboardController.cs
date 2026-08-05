using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;
using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Controllers
{
    [Authorize]
    public class VerificationDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVerificationService _verificationService;

        public VerificationDashboardController(ApplicationDbContext context, IVerificationService verificationService)
        {
            _context = context;
            _verificationService = verificationService;
        }

        protected string BaseMedicalPath
        {
            get
            {
                return Directory.Exists(@"E:\")
                    ? @"E:\Affiliation_Medical"
                    : @"D:\Affiliation_Medical";
            }
        }

        protected string BaseDentalPath
        {
            get
            {
                return Directory.Exists(@"E:\")
                    ? @"E:\Affiliation_Dental"
                    : @"D:\Affiliation_Dental";
            }
        }

        // GET: /VerificationDashboard/InstitutionDetails/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> InstitutionDetails(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
            {
                return NotFound("College code is required");
            }

            // Get institution details for the college
            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

            if (institution == null)
            {
                return NotFound($"Institution details not found for college code: {collegeCode}");
            }

            // Get college basic info
            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "Institution";
            ViewBag.UserDesignation = GetUserDesignation();

            var verification = await _verificationService.GetVerificationAsync<AffInstitutionsDetail>( x => x.CollegeCode == collegeCode, GetUserDesignation());

            ViewData["ExistingRemarks"] = verification.Remarks;

            ViewData["ExistingStatus"] = verification.IsVerified switch
            {
                true => "Approved",
                false => "Rejected",
                null => "Pending"
            };

            ViewData["ExistingStatusClass"] = verification.IsVerified switch
            {
                true => "bg-success",
                false => "bg-danger",
                null => "bg-warning"
            };

            ViewData["VerifiedBy"] = verification.VerifiedBy;

            ViewData["VerifiedDate"] =
                verification.VerifiedDate?.ToString("dd-MM-yyyy hh:mm tt");


            ViewData["ShowFeedbackForm"] = verification.IsVerified == null;

            return View(institution);
        }

        // GET: /VerificationDashboard/TrustDetails/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> TrustDetails(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
            {
                return NotFound("College code is required");
            }

            var institution = await _context.InstitutionBasicDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

            if (institution == null)
            {
                return NotFound($"Institution details not found for college code: {collegeCode}");
            }

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "TrustDetails";
            ViewBag.UserDesignation = GetUserDesignation();

            // Fetch role-based verification
            await SetVerificationViewData<InstitutionBasicDetail>(collegeCode);


            return View(institution);
        }

        // GET: /VerificationDashboard/TrustMemberDetails/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> TrustMemberDetails(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
            {
                return NotFound("College code is required");
            }

            // Get trust member details for the college from ContinuationTrustMemberDetails table
            var trustMembers = await _context.ContinuationTrustMemberDetails
                .Where(t => t.CollegeCode == collegeCode)
                .OrderBy(t => t.SlNo)
                .ToListAsync();

            // Get institution basic details for context
            var institution = await _context.InstitutionBasicDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

            // Get college basic info
            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "TrustMemberDetails";
            ViewBag.UserDesignation = GetUserDesignation();
            ViewBag.InstitutionName = institution?.NameOfInstitution ?? "Unknown Institution";


            await SetVerificationViewData<ContinuationTrustMemberDetail>(collegeCode);

            return View(trustMembers);
        }


        private async Task<IActionResult> ServeFileFromPath(int id, string facultyCode, Func<InstitutionBasicDetail, string?> pathSelector)
        {
            var entity = await _context.InstitutionBasicDetails
                .FirstOrDefaultAsync(x => x.InstitutionId == id);

            if (entity == null)
                return NotFound("Institution not found.");

            var rawPath = pathSelector(entity);

            if (string.IsNullOrWhiteSpace(rawPath))
                return NotFound("No file path stored.");

            string absolutePath;

            if (Path.IsPathRooted(rawPath.Trim()))
            {
                // Legacy absolute path support
                absolutePath = rawPath.Trim();
            }
            else
            {
                // Select root path based on faculty
                string rootPath = facultyCode == "2" ? BaseDentalPath : BaseMedicalPath;

                // Normalize slashes
                var normalized = rawPath.Trim()
                    .Replace("/", Path.DirectorySeparatorChar.ToString());

                // Build full path
                absolutePath =
                    Path.Combine(rootPath, normalized);
            }

            // File existence check
            if (!System.IO.File.Exists(absolutePath))
            {
                return NotFound(
                    $"File not found. Checked: {absolutePath}");
            }

            // Detect content type
            var ext = Path.GetExtension(absolutePath).ToLower();

            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/pdf"
            };

            // Stream inline
            var stream = new FileStream(
                absolutePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = null
            };
        }

        [HttpGet]
        public async Task<IActionResult> ViewDocument(int id, string facultyCode, string document)
        {
            var property = typeof(InstitutionBasicDetail).GetProperty(document);

            if (property == null || property.PropertyType != typeof(string))
            {
                return BadRequest("Invalid document.");
            }

            return await ServeFileFromPath(
                id,
                facultyCode,
                entity => property.GetValue(entity) as string
            );
        }

        // GET: /VerificationDashboard/DeanDirectorDetails/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> DeanDirectorDetails(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
            {
                return NotFound("College code is required");
            }

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

            if (institution == null)
            {
                return NotFound($"Institution details not found for college code: {collegeCode}");
            }

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "DeanDirectorDetails";
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<AffDeanOrDirectorDetail>(collegeCode);

            return View(institution);
        }

        private async Task SetVerificationViewData<T>(string collegeCode)  where T : class
        {
            var property = typeof(T).GetProperties()
                .FirstOrDefault(p =>
                    p.Name.Equals("CollegeCode", StringComparison.OrdinalIgnoreCase));

            if (property == null)
                throw new Exception($"{typeof(T).Name} does not contain a CollegeCode property.");

            var verification = await _verificationService
                .GetVerificationAsync<T>(
                    x => EF.Property<string>(x, property.Name) == collegeCode,
                    GetUserDesignation());

            ViewData["ExistingRemarks"] = verification.Remarks;

            ViewData["ExistingStatus"] = verification.IsVerified switch
            {
                true => "Approved",
                false => "Rejected",
                null => "Pending"
            };

            ViewData["ExistingStatusClass"] = verification.IsVerified switch
            {
                true => "bg-success",
                false => "bg-danger",
                null => "bg-warning"
            };

            ViewData["VerifiedBy"] = verification.VerifiedBy;
            ViewData["VerifiedDate"] = verification.VerifiedDate?.ToString("dd-MM-yyyy hh:mm tt");
            ViewData["ShowFeedbackForm"] = verification.IsVerified == null;
        }

        // GET: /VerificationDashboard/PrincipalDetails/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> PrincipalDetails(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
            {
                return NotFound("College code is required");
            }

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

            if (institution == null)
            {
                return NotFound($"Institution details not found for college code: {collegeCode}");
            }

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "PrincipalDetails";
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<AffPrincipalDetail>(collegeCode);

            return View(institution);
        }

        // Placeholder actions for remaining tabs (to be implemented later)
        [HttpGet]
        public async Task<IActionResult> Courses(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
                return NotFound("College code is required");

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);
            if (institution == null)
                return NotFound($"Institution not found for college code: {collegeCode}");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "Courses";
            ViewBag.ComingSoon = true;
            ViewBag.TabTitle = "Courses & Intake";
            ViewBag.TabIcon = "bi-book";
            ViewBag.UserDesignation = GetUserDesignation();

            return View("ComingSoon", institution);
        }

        // GET: /VerificationDashboard/UgCourseDetails/{collegeCode}
        //[HttpGet]
        //public async Task<IActionResult> UgCourseDetails(string collegeCode)
        //{
        //    if (string.IsNullOrEmpty(collegeCode))
        //        return NotFound("College code is required");

        //    // Get UG courses for the college (AffiliationCourseDetail table)
        //    var courses = await _context.AffiliationCourseDetails
        //        .Where(c => c.Collegecode == collegeCode)
        //        .OrderBy(c => c.CourseName)
        //        .ToListAsync();

        //    var college = await _context.AffiliationCollegeMasters
        //        .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

        //    var institution = await _context.AffInstitutionsDetails
        //        .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

        //    ViewBag.CollegeCode = collegeCode;
        //    ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
        //    ViewBag.InstitutionName = institution?.NameOfInstitution ?? "Unknown Institution";
        //    ViewBag.ActiveTab = "UgCourseDetails";
        //    ViewBag.TabTitle = "UG Course Details";
        //    ViewBag.TabIcon = "bi-book";
        //    ViewBag.UserDesignation = GetUserDesignation();
        //    ViewBag.NextTabAction = Url.Action("PgCourseDetails", new { collegeCode });
        //    ViewBag.NextTabLabel = "Next: PG Course Details";
        //    ViewBag.PrevTabAction = Url.Action("PrincipalDetails", new { collegeCode });
        //    ViewBag.PrevTabLabel = "Previous: Principal Details";

        //    return View(courses);
        //}

        // GET: /VerificationDashboard/PgCourseDetails/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> PgCourseDetails(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
                return NotFound("College code is required");

            // Get PG/SS courses for the college (AffiliationPgSsCourseDetail table)
            var courses = await _context.AffiliationPgSsCourseDetails
                .Where(c => c.CollegeCode == collegeCode)
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.InstitutionName = institution?.NameOfInstitution ?? "Unknown Institution";
            ViewBag.ActiveTab = "PgCourseDetails";
            ViewBag.TabTitle = "PG / Super Specialty Course Details";
            ViewBag.TabIcon = "bi-mortarboard";
            ViewBag.UserDesignation = GetUserDesignation();
            ViewBag.NextTabAction = Url.Action("Infrastructure", new { collegeCode });
            ViewBag.NextTabLabel = "Next: Infrastructure";
            ViewBag.PrevTabAction = Url.Action("UgCourseDetails", new { collegeCode });
            ViewBag.PrevTabLabel = "Previous: UG Course Details";

            return View(courses);
        }


        [HttpGet]
        public async Task<IActionResult> SSCourseDetails(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
            {
                return NotFound("College code is required");
            }

            var ssCourses = await _context.AffiliationPgSsCourseDetails
                .Where(x => x.CollegeCode == collegeCode && x.CourseLevel == "SS")
                .OrderBy(x => x.CourseName)
                .ToListAsync();

            if (!ssCourses.Any())
            {
                return RedirectToAction(nameof(PgCourseDetails), new { collegeCode });
            }

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "SSCourseDetails";
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<AffiliationPgSsCourseDetail>(collegeCode);

            return View(ssCourses);
        }

        [HttpGet]
        public async Task<IActionResult> Infrastructure(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
                return NotFound("College code is required");

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);
            if (institution == null)
                return NotFound($"Institution not found for college code: {collegeCode}");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "Infrastructure";
            ViewBag.ComingSoon = true;
            ViewBag.TabTitle = "Infrastructure";
            ViewBag.TabIcon = "bi-houses";

            return View("ComingSoon", institution);
        }

        [HttpGet]
        public async Task<IActionResult> Faculty(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
                return NotFound("College code is required");

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);
            if (institution == null)
                return NotFound($"Institution not found for college code: {collegeCode}");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "Faculty";
            ViewBag.ComingSoon = true;
            ViewBag.TabTitle = "Faculty Details";
            ViewBag.TabIcon = "bi-people";

            return View("ComingSoon", institution);
        }

        [HttpGet]
        public async Task<IActionResult> Clinical(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
                return NotFound("College code is required");

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);
            if (institution == null)
                return NotFound($"Institution not found for college code: {collegeCode}");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "Clinical";
            ViewBag.ComingSoon = true;
            ViewBag.TabTitle = "Clinical Material";
            ViewBag.TabIcon = "bi-hospital";

            return View("ComingSoon", institution);
        }

        [HttpGet]
        public async Task<IActionResult> Admin(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
                return NotFound("College code is required");

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);
            if (institution == null)
                return NotFound($"Institution not found for college code: {collegeCode}");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "Admin";
            ViewBag.ComingSoon = true;
            ViewBag.TabTitle = "Admin & Governance";
            ViewBag.TabIcon = "bi-person-badge";

            return View("ComingSoon", institution);
        }

        [HttpGet]
        public async Task<IActionResult> Finance(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
                return NotFound("College code is required");

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);
            if (institution == null)
                return NotFound($"Institution not found for college code: {collegeCode}");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "Finance";
            ViewBag.ComingSoon = true;
            ViewBag.TabTitle = "Finance & Fees";
            ViewBag.TabIcon = "bi-cash-stack";

            return View("ComingSoon", institution);
        }

        [HttpGet]
        public async Task<IActionResult> Documents(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
                return NotFound("College code is required");

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);
            if (institution == null)
                return NotFound($"Institution not found for college code: {collegeCode}");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.ActiveTab = "Documents";
            ViewBag.ComingSoon = true;
            ViewBag.TabTitle = "Documents";
            ViewBag.TabIcon = "bi-file-earmark-text";

            return View("ComingSoon", institution);
        }

        public static class VerificationRouteMapper
        {
            public static readonly Dictionary<string, Type> EntityMappings = new()
            {
                ["InstitutionDetails"] = typeof(AffInstitutionsDetail),
                ["TrustDetails"] = typeof(InstitutionBasicDetail),
                ["TrustMemberDetails"] = typeof(ContinuationTrustMemberDetail),
                ["DeanDirectorDetails"] = typeof(AffDeanOrDirectorDetail),
                ["PrincipalDetails"] = typeof(AffPrincipalDetail),
                ["UgCourseDetails"] = typeof(AffiliationCourseDetail),
                ["PgCourseDetails"] = typeof(AffiliationPgSsCourseDetail),
                ["Hostel_Details"] = typeof(AffHostelDetail),
                ["LandBuilding_Details"] = typeof(DentalCollegeLandBuildingDetail),
                ["TeachingStaffDepartmentWise"] = typeof(TeachingStaffDepartmentWiseDetail),
                ["AcademicIntake"] = typeof(CollegeCourseIntakeDetail)
            };
        }

        [HttpGet]
        public async Task<IActionResult> UGCourseDetails(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
            {
                return NotFound("College code is required");
            }

            var ugCourses = await _context.AffiliationCourseDetails
                .Where(x => x.Collegecode == collegeCode)
                .OrderBy(x => x.CourseName)
                .ToListAsync();

            if (!ugCourses.Any())
            {
                return NotFound($"UG Course details not found for college code: {collegeCode}");
            }

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName+", "+college?.CollegeTown ?? "Unknown College";
            ViewBag.ActiveTab = "UGCourseDetails";
            ViewBag.NextTabAction = Url.Action("PgCourseDetails", new { collegeCode });
            ViewBag.NextTabLabel = "Next: PG Course Details";
            ViewBag.PrevTabAction = Url.Action("PrincipalDetails", new { collegeCode });
            ViewBag.PrevTabLabel = "Previous: Principal Details";
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<AffiliationCourseDetail>(collegeCode);

            return View(ugCourses);
        }

        // POST: Save verification remarks and status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVerification(string collegeCode, string tabName, string remarks, string status)
        {
            try
            {
                var request = new VerificationRequest
                {
                    Role = GetUserDesignation(),
                    Status = status,
                    Remarks = remarks,
                    VerifiedBy = User.Identity?.Name ?? GetUserDesignation()
                };

                if (!VerificationRouteMapper.EntityMappings.TryGetValue(tabName, out var entityType))
                {
                    TempData["ErrorMessage"] = $"Verification is not configured for '{tabName}'.";
                    return RedirectToAction(tabName, new { collegeCode });
                }

                var verificationHandlers = new Dictionary<string, Func<Task>>
                {
                    ["InstitutionDetails"] = () =>
                        _verificationService.SaveVerificationAsync<AffInstitutionsDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["TrustDetails"] = () =>
                        _verificationService.SaveVerificationAsync<InstitutionBasicDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["PrincipalDetails"] = () =>
                        _verificationService.SaveVerificationAsync<AffPrincipalDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["TrustMemberDetails"] = () =>
                        _verificationService.SaveVerificationAsync<ContinuationTrustMemberDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["DeanDirectorDetails"] = () =>
                        _verificationService.SaveVerificationAsync<AffDeanOrDirectorDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["UgCourseDetails"] = () =>
                        _verificationService.SaveVerificationAsync<AffiliationCourseDetail>(
                            x => x.Collegecode == collegeCode,
                            request),

                    ["PgCourseDetails"] = () =>
                        _verificationService.SaveVerificationAsync<AffiliationPgSsCourseDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    // Continue adding the remaining tabs...
                };

                if (!verificationHandlers.TryGetValue(tabName, out var handler))
                {
                    TempData["ErrorMessage"] = $"Verification is not configured for '{tabName}'.";
                    return RedirectToAction(tabName, new { collegeCode });
                }

                await handler();

                TempData["SuccessMessage"] = "Verification saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(tabName, new { collegeCode });
        }


        // Helper method to get current user's designation
        private string GetUserDesignation()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var user = _context.TblRguhsFacultyUsers.FirstOrDefault(u => u.Id == userId);
                return user?.DesignationDescription ?? "Unknown";
            }
            return "Unknown";
        }
    }
}