using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification;
using VerificationPortal.Services.Verification.Interfaces;
using VerificationPortal.Services.Verification.Models;
using VerificationPortal.ViewModels;

namespace VerificationPortal.Controllers
{
    [Authorize]
    public class VerificationDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVerificationService _verificationService;
        private readonly IClinicalFacilitiesCompositeService _clinicalFacilitiesCompositeService;
        private static readonly int?[] _yearIds = { 1, 2, 3, 4 };

        public VerificationDashboardController(ApplicationDbContext context, IVerificationService verificationService, IClinicalFacilitiesCompositeService clinicalFacilitiesCompositeService)
        {
            _context = context;
            _verificationService = verificationService;
            _clinicalFacilitiesCompositeService = clinicalFacilitiesCompositeService;
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

        protected async Task<List<string>> GetSortedCourseLevels(string collegeCode)
        {

            var order = new List<string> { "UG", "PG", "SS" };

            var levels = await (
                from ai in _context.AcademicIntakes
                join mc in _context.MstCourses
                    on ai.Courses equals mc.CourseCode.ToString()
                where ai.CollegeCode == collegeCode && ai.Ay2026TotalIntake > 0
                      && !string.IsNullOrEmpty(ai.Courses)
                select mc.CourseLevel
            )
            .Distinct()
            .ToListAsync();

            levels = levels
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim().ToUpper())
                .Distinct()
                .OrderBy(l => order.Contains(l)
                    ? order.IndexOf(l)
                    : int.MaxValue)
                .ThenBy(l => l)
                .ToList();

            // fallback
            if (!levels.Any())
            {
                levels.Add("UG");
            }

            return levels;
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

        [HttpGet]
        public async Task<IActionResult> ChairDistribution(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
            {
                return NotFound("College code is required.");
            }

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            if (institution == null)
            {
                return NotFound($"Institution details not found for college code: {collegeCode}");
            }

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            int facultyCode = Convert.ToInt32(institution.FacultyCode);

            var academicIntakes = await _context.AcademicIntakes
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode.ToString())
                .ToListAsync();

            var savedDentalChairs = await _context.DentalChairs
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode)
                .ToListAsync();

            var hospital = await _context.HospitalDetailsForAffiliations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            bool collegeCourseExists = await _context.CollegeCourseIntakeDetails
                .AnyAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode);

            List<DentalChairVm> model = new();

            // Existing logic from your Affiliation controller
            if (collegeCourseExists)
            {
                var courses = await (
                    from mc in _context.MstCourses
                    join cc in _context.CollegeCourseIntakeDetails
                        on mc.CourseCode.ToString() equals cc.CourseCode
                    where mc.FacultyCode == facultyCode
                       && cc.CollegeCode == collegeCode
                       && cc.FacultyCode == facultyCode
                    select mc
                ).Distinct().ToListAsync();

                foreach (var course in courses)
                {
                    var intake = academicIntakes.FirstOrDefault(x =>
                        !string.IsNullOrEmpty(x.Courses) &&
                        x.Courses.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(c => c.Trim())
                            .Contains(course.CourseCode.ToString()));

                    int seatCount = intake?.Ay2025TotalIntake ?? 0;

                    if (seatCount <= 0)
                        continue;

                    int seatSlab = ((seatCount - 1) / 50 + 1) * 50;

                    var slab = await _context.SeatSlabMasters
                        .FirstOrDefaultAsync(x =>
                            x.FacultyCode == facultyCode &&
                            x.SeatSlab == seatSlab);

                    var existing = savedDentalChairs
                        .FirstOrDefault(x => x.CourseCode == course.CourseCode);

                    model.Add(new DentalChairVm
                    {
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        CourseLevel = course.CourseLevel,
                        SeatSlab = seatSlab,
                        HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                        SeatSlabId = slab?.SeatSlabId ?? "",
                        ChairsRequired = seatSlab,
                        ChairsExisting = existing?.ChairsExisting ?? 0
                    });
                }
            }
            else
            {
                var courseCodes = academicIntakes
                    .Where(x => !string.IsNullOrEmpty(x.Courses))
                    .SelectMany(x => x.Courses!
                        .Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Select(x => x.Trim())
                    .Distinct()
                    .ToList();

                var courses = await _context.MstCourses
                    .Where(x =>
                        x.FacultyCode == facultyCode &&
                        courseCodes.Contains(x.CourseCode.ToString()))
                    .ToListAsync();

                foreach (var course in courses)
                {
                    var intake = academicIntakes.FirstOrDefault(x =>
                        !string.IsNullOrEmpty(x.Courses) &&
                        x.Courses.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(c => c.Trim())
                            .Contains(course.CourseCode.ToString()));

                    int seatCount = intake?.Ay2025TotalIntake ?? 0;

                    if (seatCount <= 0)
                        continue;

                    int seatSlab = ((seatCount - 1) / 50 + 1) * 50;

                    var slab = await _context.SeatSlabMasters
                        .FirstOrDefaultAsync(x =>
                            x.FacultyCode == facultyCode &&
                            x.SeatSlab == seatSlab);

                    var existing = savedDentalChairs
                        .FirstOrDefault(x => x.CourseCode == course.CourseCode);

                    model.Add(new DentalChairVm
                    {
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        CourseLevel = course.CourseLevel,
                        SeatSlab = seatSlab,
                        HospitalDetailsId = hospital?.HospitalDetailsId ?? 0,
                        SeatSlabId = slab?.SeatSlabId ?? "",
                        ChairsRequired = seatSlab,
                        ChairsExisting = existing?.ChairsExisting ?? 0
                    });
                }
            }

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.InstitutionName = institution.NameOfInstitution ?? "Unknown Institution";
            ViewBag.ActiveTab = "ChairDistribution";
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<DentalChair>(collegeCode);

            return View(model);
        }

        private async Task SetVerificationViewData<T>(string collegeCode) where T : class
        {
            var property = typeof(T).GetProperties()
                .FirstOrDefault(p =>
                    p.Name.Equals(
                        "CollegeCode",
                        StringComparison.OrdinalIgnoreCase));

            if (property == null)
                throw new Exception(
                    $"{typeof(T).Name} does not contain a CollegeCode property.");

            var verification = await _verificationService
                .GetVerificationAsync<T>(
                    x => EF.Property<string>(x, property.Name) == collegeCode,
                    GetUserDesignation());

            if (verification == null)
            {
                ViewData["ExistingRemarks"] = null;
                ViewData["ExistingStatus"] = "Pending";
                ViewData["ExistingStatusClass"] = "bg-warning";
                ViewData["VerifiedBy"] = null;
                ViewData["VerifiedDate"] = null;
                ViewData["ShowFeedbackForm"] = true;

                return;
            }

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

            ViewData["ShowFeedbackForm"] =
                verification.IsVerified == null;
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

        // GET: /VerificationDashboard/Infrastructure/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> Infrastructure(string collegeCode)
        {
            if (string.IsNullOrEmpty(collegeCode))
                return NotFound("College code is required");

            // Get college basic info
            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            // Get institution basic info
            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

            // Get faculty code from institution
            var facultyCodeStr = institution?.FacultyCode ?? "0";
            int facultyCode = int.TryParse(facultyCodeStr, out int fc) ? fc : 0;

            // Get academic intake to determine seat intake
            var academicIntake = await _context.AcademicIntakes
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode && x.FacultyCode == facultyCodeStr);

            if (academicIntake == null)
            {
                TempData["Error"] = "Academic intake details not found.";
                return RedirectToAction("PgCourseDetails", new { collegeCode });
            }

            // Hospital Details (Prerequisite)
            var hospital = await _context.HospitalDetailsForAffiliations
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode && x.FacultyCode == facultyCodeStr);

            if (hospital == null)
            {
                TempData["Warning"] = "Please complete Hospital Details before proceeding.";
            }

            // Seat Intake - use the latest academic year
            int seatIntake = academicIntake.Ay2026TotalIntake > 0 ? academicIntake.Ay2026TotalIntake :
                            (academicIntake.Ay2025TotalIntake > 0 ? academicIntake.Ay2025TotalIntake :
                            academicIntake.Ay2024TotalIntake);

            // Seat Slab calculation
            int seatSlab = GetSeatSlab(seatIntake);

            // Fetch Master Norms
            var slabNorm = await _context.UgSeatSlabNormMasters
                .FirstOrDefaultAsync(x => x.FacultyCode == facultyCode && x.SeatSlab == seatSlab);

            if (slabNorm == null)
            {
                TempData["Error"] = "Seat slab norms not configured.";
                return RedirectToAction("PgCourseDetails", new { collegeCode });
            }

            // Master Infrastructure Requirements
            var infraMasters = await _context.MstDentalInfrastructures
                .Where(x => x.FacultyCode == facultyCode && x.SeatSlab == seatSlab)
                .OrderBy(x => x.SlNo)
                .ToListAsync();

            // Existing Saved Infrastructure
            var savedInfrastructure = await _context.DentalInfrastructures
                .Where(x => x.CollegeCode == collegeCode && x.FacultyCode == facultyCode && x.SeatSlab == seatSlab)
                .Include(x => x.Requirement)
                .Include(x => x.HospitalDetails)
                .ToListAsync();

            // Build ViewModel combining master requirements with saved data
            var infraViewModel = infraMasters.Select(m =>
            {
                var saved = savedInfrastructure.FirstOrDefault(x => x.RequirementId == m.Id);
                return new DentalInfrastructure
                {
                    Id = saved?.Id ?? 0,
                    FacultyCode = facultyCode,
                    AffiliationTypeId = saved?.AffiliationTypeId ?? 0,
                    CollegeCode = collegeCode,
                    HospitalDetailsId = saved?.HospitalDetailsId ?? hospital?.HospitalDetailsId ?? 0,
                    RequirementId = m.Id,
                    SeatSlab = seatSlab,
                    RequiredAreaSqFt = m.RequiredAreaSqFt,
                    AvailableAreaSqFt = saved?.AvailableAreaSqFt ?? 0,
                    CreatedOn = saved?.CreatedOn ?? DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                    CourseLevel = saved?.CourseLevel,
                    Requirement = m,
                    HospitalDetails = saved?.HospitalDetails ?? hospital
                };
            }).ToList();

            // Get Medical Skills Laboratory data
            var medicalSkillsLab = await _context.MedicalSkillsLaboratories
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode && x.FacultyCode == facultyCodeStr);

            // Get Pre-Clinical & Skills Lab Area Requirements (master)
            var preClinicalMasterReqs = await _context.MstDentalPreClinicalAndSkillsLaboratoryAreaReqs
                .Where(x => x.FacultyCode == facultyCode && x.SeatIntake == seatSlab)
                .OrderBy(x => x.SectionCode)
                .ThenBy(x => x.LaboratoryName)
                .ToListAsync();

            // Get existing saved pre-clinical lab requirements
            var savedPreClinicalReqs = await _context.DentalPreClinicalAndSkillsLabAreaReqs
                .Where(x => x.CollegeCode == collegeCode && x.FacultyCode == facultyCode && x.SeatIntake == seatSlab)
                .Include(x => x.Lab)
                .ToListAsync();

            // Build pre-clinical lab view model combining master with saved data
            var preClinicalViewModel = preClinicalMasterReqs.Select(m =>
            {
                var saved = savedPreClinicalReqs.FirstOrDefault(x => x.LabId == m.Id);
                return new DentalPreClinicalAndSkillsLabAreaReq
                {
                    Id = saved?.Id ?? 0,
                    CollegeCode = collegeCode,
                    FacultyCode = facultyCode,
                    SeatIntake = seatSlab,
                    LabId = m.Id,
                    LabName = m.LaboratoryName,
                    RequiredAreaSqFt = m.AreaRequiredSqFt,
                    ExistingAreaSqFt = saved?.ExistingAreaSqFt ?? 0,
                    IsActive = m.IsActive,
                    CreatedOn = saved?.CreatedOn ?? DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    Lab = m
                };
            }).ToList();

            // Get Dental College Land & Building Details
            var landBuildingDetail = await _context.DentalCollegeLandBuildingDetails
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode && x.FacultyCode == facultyCode && x.SeatSlab == seatSlab);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.InstitutionName = institution?.NameOfInstitution ?? "Unknown Institution";
            ViewBag.ActiveTab = "Infrastructure";
            ViewBag.TabTitle = "Classroom & Laboratory";
            ViewBag.TabIcon = "bi-houses";
            ViewBag.UserDesignation = GetUserDesignation();
            ViewBag.NextTabAction = Url.Action("Faculty", new { collegeCode });
            ViewBag.NextTabLabel = "Next: Faculty Details";
            ViewBag.PrevTabAction = Url.Action("PgCourseDetails", new { collegeCode });
            ViewBag.PrevTabLabel = "Previous: PG Course Details";
            ViewBag.SeatIntake = seatIntake;
            ViewBag.SeatSlab = seatSlab;
            ViewBag.SlabNorm = slabNorm;
            ViewBag.MedicalSkillsLaboratory = medicalSkillsLab;
            ViewBag.PreClinicalLabRequirements = preClinicalViewModel;
            ViewBag.LandBuildingDetail = landBuildingDetail;

            await SetVerificationViewData<DentalInfrastructure>(collegeCode);

            return View(infraViewModel);
        }

        // Helper method to calculate seat slab based on intake
        private int GetSeatSlab(int seatIntake)
        {
            return seatIntake switch
            {
                <= 50 => 50,
                <= 100 => 100,
                <= 150 => 150,
                <= 200 => 200,
                <= 250 => 250,
                <= 300 => 300,
                _ => 300
            };
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
            public static readonly Dictionary<string, Type> EntityMappings = new(StringComparer.OrdinalIgnoreCase)
            {
                { "InstitutionDetails", typeof(AffInstitutionsDetail) },
                { "TrustDetails", typeof(InstitutionBasicDetail) },
                { "TrustMemberDetails", typeof(ContinuationTrustMemberDetail) },
                { "DeanDirectorDetails", typeof(AffDeanOrDirectorDetail) },
                { "PrincipalDetails", typeof(AffPrincipalDetail) },
                { "UgCourseDetails", typeof(AffiliationCourseDetail) },
                { "PgCourseDetails", typeof(AffiliationPgSsCourseDetail) },
                { "LandAndBuildingDetails", typeof(DentalCollegeLandBuildingDetail) },
                { "ChairDistribution", typeof(DentalChair) },
                { "BedDistribution", typeof(MedicalUgbedDistribution) },

                { "HostelDetails", typeof(AffHostelDetail) },

                { "DepartmentOfficesAndEducationalUnit", typeof(MedicalDepartmentOfficesMeu) },

                { "EquipmentList", typeof(DentalCollegeEquipmentDetail) },

                { "CAVehicleDetails", typeof(CaVehicleDetail) },

                { "UgAcademicMatters", typeof(CaAcademicPerformance) },

                { "PgAcademicMatters",typeof(CaAcademicPerformance) },

                { "FinanceDetails", typeof(MedCaAccountAndFeeDetail) },

                { "StaffPayScale", typeof(MedCaStaffParticular) },
                { "StaffOtherDetails", typeof(CaMedStaffParticularsOther) },
                { "LibraryServices", typeof(CaMedicalLibraryService) },

                { "LibraryDetails", typeof(CaMedLibraryGeneral) },

                { "ClinicalFacilities", typeof(HospitalDetailsForAffiliation) },

                { "FacultyDetails", typeof(FacultyDetail) },

                { "TeachingExperience", typeof(TeachingStaffDepartmentWiseDetail) },

                { "ClassroomAndLaboratory", typeof(DentalInfrastructure) },
                { "TeachingStaffDepartmentWise", typeof(TeachingStaffDepartmentWiseDetail) },
                { "AcademicIntake", typeof(CollegeCourseIntakeDetail) },
                { "ResearchPublications", typeof(CaMedResearchPublicationsDetail) }
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

        [HttpGet]
        public async Task<IActionResult> LandAndBuildingDetails(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

            var landBuilding = await _context.DentalCollegeLandBuildingDetails
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            if (landBuilding == null)
                return NotFound($"Land & Building details not found for college code: {collegeCode}");

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.InstitutionName = institution?.NameOfInstitution ?? "Unknown Institution";
            ViewBag.ActiveTab = "LandAndBuildingDetails";
            ViewBag.UserDesignation = GetUserDesignation();

            var verification = await _verificationService.GetVerificationAsync<DentalCollegeLandBuildingDetail>(
                x => x.CollegeCode == collegeCode,
                GetUserDesignation());

            ViewData["ExistingRemarks"] = verification?.Remarks ?? "";
            ViewData["ExistingStatus"] = verification?.IsVerified switch
            {
                true => "Approved",
                false => "Rejected",
                _ => "Pending"
            };

            ViewData["ExistingStatusClass"] = verification?.IsVerified switch
            {
                true => "bg-success",
                false => "bg-danger",
                _ => "bg-warning"
            };

            ViewData["VerifiedBy"] = verification?.VerifiedBy ?? "";
            ViewData["VerifiedDate"] = verification?.VerifiedDate?.ToString("dd-MM-yyyy hh:mm tt") ?? "";

            // Only show the form if there is NO verification at all
            ViewData["ShowFeedbackForm"] = verification == null;

            return View(landBuilding);
        }

        [HttpGet]
        public async Task<IActionResult> ClassroomAndLaboratory(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(i => i.CollegeCode == collegeCode);

            if (institution == null)
                return NotFound($"Institution not found for college code: {collegeCode}");

                if (string.IsNullOrWhiteSpace(institution.FacultyCode))
                return NotFound("Faculty code not found.");

            int facultyCode = Convert.ToInt32(institution.FacultyCode);

            var academicIntake = await _context.AcademicIntakes
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode.ToString());

            if (academicIntake == null)
            {
                TempData["Error"] = "Academic intake details not found.";
                return RedirectToAction(nameof(PgCourseDetails), new { collegeCode });
            }

            int seatIntake = academicIntake.Ay2026TotalIntake;
            int seatSlab = GetSeatSlab(seatIntake);

            var infraMasters = await _context.MstDentalInfrastructures
                .Where(x => x.FacultyCode == facultyCode &&
                            x.SeatSlab == seatSlab)
                .OrderBy(x => x.SlNo)
                .ToListAsync();

            var savedInfrastructure = await _context.DentalInfrastructures
                .Where(x => x.CollegeCode == collegeCode &&
                            x.FacultyCode == facultyCode &&
                            x.SeatSlab == seatSlab)
                .ToListAsync();

            var skillsLab = await _context.MedicalSkillsLaboratories
                .Include(x => x.AffiliationType)
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            //var preClinicalLabs = await _context.DentalPreClinicalAndSkillsLabAreaReqs
            //    .Include(x => x.Lab)
            //    .Where(x => x.CollegeCode == collegeCode)
            //    .OrderBy(x => x.Lab.LaboratoryName)
            //    .ToListAsync();

            // ADD THIS
            var landBuilding = await _context.DentalCollegeLandBuildingDetails
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.InstitutionName = institution.NameOfInstitution ?? "Unknown Institution";
            ViewBag.ActiveTab = "ClassroomAndLaboratory";
            ViewBag.UserDesignation = GetUserDesignation();

            var verification = await _verificationService.GetVerificationAsync<DentalInfrastructure>(
                x => x.CollegeCode == collegeCode,
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

            ViewData["VerifiedDate"] =
                verification.VerifiedDate?.ToString("dd-MM-yyyy hh:mm tt");

            ViewData["ShowFeedbackForm"] = verification.IsVerified == null;

            var model = new ClassroomAndLaboratoryViewModel
            {
                CollegeCode = collegeCode,
                FacultyCode = facultyCode,
                SeatIntake = seatIntake,
                SeatSlab = seatSlab,

                MedicalSkillsLaboratory = skillsLab,

                //PreClinicalLabRequirements = preClinicalLabs,
                LandBuildingDetails = landBuilding,

                InfrastructureDetails = infraMasters.Select(m =>
                {
                    var saved = savedInfrastructure
                        .FirstOrDefault(x => x.RequirementId == m.Id);

                    return new DentalInfrastructureVM
                    {
                        Id = saved?.Id ?? 0,
                        RequirementId = m.Id,
                        SlNo = m.SlNo,
                        RequirementName = m.RequirementName,
                        RequirementDescription = m.RequirementDescription,
                        SeatSlab = m.SeatSlab,
                        RequiredAreaSqFt = m.RequiredAreaSqFt,
                        AvailableAreaSqFt = saved?.AvailableAreaSqFt ?? 0
                    };
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EquipmentList(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            if (institution == null)
                return NotFound($"Institution not found for college code: {collegeCode}");

            if (string.IsNullOrWhiteSpace(institution.FacultyCode))
                return NotFound("Faculty code not found.");

            var facultyCode = Convert.ToInt32(institution.FacultyCode);

            // Department master
            var departments = await _context.MstEquipmentDepartments
                .Where(x =>
                    x.FacultyCode == facultyCode &&
                    x.IsActive)
                .ToListAsync();

            var departmentLookup = departments
                .GroupBy(x => x.DepartmentCode)
                .ToDictionary(
                    x => x.Key,
                    x => x.First().DepartmentName
                );

            // Master equipment
            var masterEquipment = await _context.MstEquipmentDeptWises
                .Where(x =>
                    x.FacultyCode == facultyCode &&
                    x.IsActive)
                .OrderBy(x => x.DepartmentCode)
                .ThenBy(x => x.EquipmentName)
                .ToListAsync();

            // Existing college equipment
            var savedEquipment = await _context.DentalCollegeEquipmentDetails
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode &&
                    x.IsActive)
                .ToListAsync();

            var savedLookup = savedEquipment
                .GroupBy(x => x.EquipmentId)
                .ToDictionary(
                    x => x.Key,
                    x => x.First()
                );

            var equipments = masterEquipment
                .Select(item =>
                {
                    savedLookup.TryGetValue(item.Id, out var saved);

                    departmentLookup.TryGetValue(
                        item.DepartmentCode,
                        out var departmentName);

                    return new EquipmentRowVM
                    {
                        EquipmentId = item.Id,

                        DepartmentCode = item.DepartmentCode,

                        DepartmentName =
                            departmentName ?? item.DepartmentCode,

                        EquipmentName = item.EquipmentName,

                        Specification = item.Specification,

                        OneUnitReq = item.OneUnitRequirement,

                        TwoUnitReq = item.TwoUnitRequirement,

                        OneUnitExisting = saved?.OneUnitExisting,

                        TwoUnitExisting = saved?.TwoUnitExisting
                    };
                })
                .ToList();

            var vm = new EquipmentPageVM
            {
                CollegeCode = collegeCode,
                FacultyCode = facultyCode,
                Equipments = equipments
            };

            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = facultyCode;
            ViewBag.InstitutionName =
                institution.NameOfInstitution ?? "Unknown Institution";

            ViewBag.ActiveTab = "EquipmentList";
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<DentalCollegeEquipmentDetail>(
                collegeCode);

            return View(vm);
        }
        private async Task<VerificationPageContext> GetPageContextAsync(string collegeCode)
        {
            var institution = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            if (institution == null)
                throw new Exception($"Institution not found for college code '{collegeCode}'.");

            if (string.IsNullOrWhiteSpace(institution.FacultyCode))
                throw new Exception("Faculty code not found.");

            return new VerificationPageContext
            {
                Institution = institution
            };
        }

        private void PopulateCommonViewBags(VerificationPageContext context)
        {
            ViewBag.InstitutionName = context.InstitutionName;
            ViewBag.CollegeCode = context.CollegeCode;
            ViewBag.FacultyCode = context.FacultyCode;
        }

        [HttpGet]
        public async Task<IActionResult> BedDistribution(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            var college = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            var context = await GetPageContextAsync(collegeCode);

            PopulateCommonViewBags(context);

            int facultyCode = context.FacultyCodeInt;

            var existing = await _context.MedicalUgbedDistributions
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode.ToString());

            var vm = new MedicalUGBedDistributionVm();

            // =========================================
            // MEDICAL DATA
            // =========================================

            if (existing != null)
            {
                vm.Id = existing.Id;

                vm.GenMedicine = existing.GenMedicine;
                vm.Paediatrics = existing.Paediatrics;
                vm.SkinVD = existing.SkinVd;
                vm.Psychiatry = existing.Psychiatry;

                vm.GenSurgery = existing.GenSurgery;
                vm.Orthopaedics = existing.Orthopaedics;
                vm.Ophthalmology = existing.Ophthalmology;
                vm.ENT = existing.Ent;

                vm.ObstetricsANC = existing.ObstetricsAnc;
                vm.Gynaecology = existing.Gynaecology;
                vm.Postpartum = existing.Postpartum;

                vm.MajorOT = existing.MajorOt;
                vm.MinorOT = existing.MinorOt;

                vm.ICCU = existing.Iccu;
                vm.ICU = existing.Icu;
                vm.PICU_NICU = existing.PicuNicu;
                vm.SICU = existing.Sicu;
                vm.TotalICUBeds = existing.TotalIcubeds;
                vm.CasualtyBeds = existing.CasualtyBeds;

                vm.OralMaxillofacialSurgery = existing.OralMaxillofacialSurgery;
            }


            // =========================================
            // VIEW DATA
            // =========================================

            ViewBag.FacultyCode = facultyCode;
            ViewBag.CollegeCode = collegeCode;
            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";
            ViewBag.InstitutionName = context.InstitutionName ?? "Unknown Institution";
            ViewBag.ActiveTab = "BedDistribution";
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<MedicalUgbedDistribution>(collegeCode);

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> HostelDetails(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            var context = await GetPageContextAsync(collegeCode);

            PopulateCommonViewBags(context);

            ViewBag.ActiveTab = "HostelDetails";
            ViewBag.UserDesignation = GetUserDesignation();

            var college = await _context.AffiliationCollegeMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CollegeCode == collegeCode);

            ViewBag.CollegeName = college?.CollegeName ?? "Unknown College";

            var hostel = await _context.AffHostelDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == context.FacultyCode);

            if (hostel != null && !string.IsNullOrWhiteSpace(hostel.OwnOrRented))
                hostel.OwnOrRented = hostel.OwnOrRented.Trim();

            var vm = new AffHostelDetailsCreateVm
            {
                Hostel = hostel ?? new AffHostelDetail
                {
                    CollegeCode = collegeCode,
                    FacultyCode = context.FacultyCode
                }
            };

            await SetVerificationViewData<AffHostelDetail>(collegeCode);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DepartmentOfficesAndEducationalUnit(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction(nameof(Index));

            // Get common verification context
            var context = await GetPageContextAsync(collegeCode);

            // Populate common ViewBags
            PopulateCommonViewBags(context);

            ViewBag.ActiveTab = "DepartmentOfficesAndEducationalUnit";
            ViewBag.UserDesignation = GetUserDesignation();

            // Use FacultyCode from VerificationPageContext
            var facultyCode = context.FacultyCode;

            var entity = await _context.MedicalDepartmentOfficesMeus
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == context.CollegeCode &&
                    x.FacultyCode == facultyCode);

            if (entity == null)
                return NotFound();

            var vm = new DepartmentOfficesMeuViewModel
            {
                CourseLevel = entity.CourseLevel,

                // Common Department / Office details
                HasHodRoomWithOfficeAndRecords =
                    entity.HasHodRoomWithOfficeAndRecords,

                HasRoomsForFacultyAndResidents =
                    entity.HasRoomsForFacultyAndResidents,

                FacultyRoomsHaveCommunicationComputerInternet =
                    entity.FacultyRoomsHaveCommunicationComputerInternet,

                HasRoomsForNonTeachingStaff =
                    entity.HasRoomsForNonTeachingStaff
            };

            // ==========================================
            // DENTAL EDUCATION UNIT
            // ==========================================
            if (facultyCode == "2")
            {
                vm.HasDentalEducationUnit =
                    entity.HasDentalEducationUnit;

                vm.DentalEducationUnitAreaSqm =
                    entity.DentalEducationUnitAreaSqm;

                vm.DentalEducationUnitHasAudioVisual =
                    entity.DentalEducationUnitHasAudioVisual;

                vm.DentalEducationUnitHasInternet =
                    entity.DentalEducationUnitHasInternet;

                vm.DeuCoordinatorName =
                    entity.DeuCoordinatorName;

                vm.DeuCoordinatorDesignationDepartment =
                    entity.DeuCoordinatorDesignationDepartment;

                vm.DeuCoordinatorPhone =
                    entity.DeuCoordinatorPhone;

                vm.DeuCoordinatorEmail =
                    entity.DeuCoordinatorEmail;

                vm.DeuActivitiesLastAcademicYear =
                    entity.DeuActivitiesLastAcademicYear;

                vm.HasDeuMembersListFile =
                    !string.IsNullOrWhiteSpace(entity.DeuMembersListFilePath);
            }

            // ==========================================
            // MEDICAL EDUCATION UNIT
            // ==========================================
            else
            {
                vm.HasMedicalEducationUnit =
                    entity.HasMedicalEducationUnit;

                vm.MedicalEducationUnitAreaSqm =
                    entity.MedicalEducationUnitAreaSqm;

                vm.MedicalEducationUnitHasAudioVisual =
                    entity.MedicalEducationUnitHasAudioVisual;

                vm.MedicalEducationUnitHasInternet =
                    entity.MedicalEducationUnitHasInternet;

                vm.MeuCoordinatorName =
                    entity.MeuCoordinatorName;

                vm.MeuCoordinatorDesignationDepartment =
                    entity.MeuCoordinatorDesignationDepartment;

                vm.MeuCoordinatorPhone =
                    entity.MeuCoordinatorPhone;

                vm.MeuCoordinatorEmail =
                    entity.MeuCoordinatorEmail;

                vm.MeuActivitiesLastAcademicYear =
                    entity.MeuActivitiesLastAcademicYear;

                vm.HasMeuMembersListFile =
                    !string.IsNullOrWhiteSpace(entity.MeuMembersListFilePath);
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ViewDeuMembersList(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            // Get common verification context
            var context = await GetPageContextAsync(collegeCode);

            var facultyCode = context.FacultyCode;

            var entity = await _context.MedicalDepartmentOfficesMeus
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == context.CollegeCode &&
                    x.FacultyCode == facultyCode);

            if (entity == null)
                return NotFound("Department / Educational Unit details not found.");

            if (string.IsNullOrWhiteSpace(entity.DeuMembersListFilePath))
                return NotFound("DEU Members List file not found.");

            if (!System.IO.File.Exists(entity.DeuMembersListFilePath))
                return NotFound("DEU Members List file does not exist.");

            // Open PDF directly in browser
            Response.Headers["Content-Disposition"] = "inline";

            return PhysicalFile(
                entity.DeuMembersListFilePath,
                "application/pdf"
            );
        }

        private async Task PopulateCommonViewBags(string collegeCode)
        {
            var institution = await _context.AffInstitutionsDetails
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = HttpContext.Session.GetString("FacultyCode");

            if (institution != null)
            {
                ViewBag.InstitutionName = institution.NameOfInstitution;
            }

            ViewBag.UserDesignation = HttpContext.Session.GetString("UserDesignation");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPossessionProof(string collegeCode, string facultyCode)
        {
            var hostel = await _context.AffHostelDetails
                .FirstOrDefaultAsync(h => h.CollegeCode == collegeCode && h.FacultyCode == facultyCode);

            if (hostel == null ||
                string.IsNullOrEmpty(hostel.PossessionProofPath) ||
                !System.IO.File.Exists(hostel.PossessionProofPath))
                return NotFound("File not found");

            // 🔥 INLINE VIEW (NOT DOWNLOAD)
            Response.Headers["Content-Disposition"] = "inline";

            return PhysicalFile(hostel.PossessionProofPath, "application/pdf");
        }


        [HttpGet]
        public async Task<IActionResult> CA_VehicleDetails(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            // Get institution information
            var pageContext = await GetPageContextAsync(collegeCode);

            var institution = pageContext.Institution;
            var facultyCode = institution.FacultyCode!.Trim();


            // Get vehicle details
            var vehicles = await _context.CaVehicleDetails
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode)
                .OrderBy(x => x.Id)
                .ToListAsync();

            // Map to ViewModel
            var vm = new CA_VehicleDetailsViewModel
            {
                CollegeCode = collegeCode,
                FacultyCode = facultyCode,
                RegistrationNo = vehicles
                    .FirstOrDefault()?.RegistrationNo,

                ExistingList = vehicles.Select(x => new CA_VehicleDetailsViewModel
                {
                    Id = x.Id,
                    CollegeCode = x.CollegeCode,
                    FacultyCode = x.FacultyCode,
                    RegistrationNo = x.RegistrationNo,

                    VehicleRegNo = x.VehicleRegNo,
                    VehicleForCode = x.VehicleForCode,
                    SeatingCapacity = x.SeatingCapacity,

                    // DateOnly -> DateTime for ViewModel
                    ValidityDate = x.ValidityDate.HasValue
                        ? x.ValidityDate.Value.ToDateTime(TimeOnly.MinValue)
                        : null,

                    RcBookStatus = x.RcBookStatus,
                    InsuranceStatus = x.InsuranceStatus,
                    DrivingLicenseStatus = x.DrivingLicenseStatus

                }).ToList()
            };

            // Common verification page data
            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = facultyCode;
            ViewBag.InstitutionName = institution.NameOfInstitution ?? "Unknown Institution";
            ViewBag.UserDesignation = GetUserDesignation();
            ViewBag.ActiveTab = "CAVehicleDetails";

            await SetVerificationViewData<CaVehicleDetail>(collegeCode);

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> UgAcademicMatters(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            // Common verification page context
            var pageContext = await GetPageContextAsync(collegeCode);

            var institution = pageContext.Institution;

            if (string.IsNullOrWhiteSpace(institution.FacultyCode))
                return NotFound("Faculty code not found.");

            if (!int.TryParse(institution.FacultyCode.Trim(), out int facultyId))
                return NotFound("Invalid faculty code.");

            var affiliationType =
                HttpContext.Session.GetInt32("AffiliationType") ?? 2;

            var model = new CA_Aff_AcademicMattersViewModel
            {
                CollegeCode = collegeCode,
                FacultyId = facultyId,
                AffiliationType = affiliationType
            };

            // ============================================================
            // 1. ACADEMIC PERFORMANCE
            // ============================================================

            var academics = await _context.CaAcademicPerformances
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyId == facultyId &&
                    x.AffiliationType == affiliationType &&
                    x.YearOfStudyId != null &&
                    _yearIds.Contains(x.YearOfStudyId.Value))
                .ToListAsync();

            var yearMaster = await _context.CaMstYearOfStudies
                .AsNoTracking()
                .Where(y => _yearIds.Contains(y.YearOfStudyId))
                .ToDictionaryAsync(
                    y => y.YearOfStudyId,
                    y => y.YearName);

            var academicRows = new List<AcademicPerformanceViewModel>();

            foreach (var yearId in _yearIds)
            {
                if (!yearId.HasValue)
                    continue;

                var existing = academics
                    .FirstOrDefault(x => x.YearOfStudyId == yearId.Value);

                academicRows.Add(new AcademicPerformanceViewModel
                {
                    AcademicPerformanceId =
                        existing?.AcademicPerformanceId ?? 0,

                    YearOfStudyId = yearId.Value,

                    YearName = yearMaster.TryGetValue(
                        yearId.Value,
                        out var yearName)
                            ? yearName
                            : "",

                    RegularStudents = existing?.RegularStudents,
                    RepeaterStudents = existing?.RepeaterStudents,
                    NumberOfStudentsPassed = existing?.NumberOfStudentsPassed,
                    PassPercentage = existing?.PassPercentage,
                    FirstClassCount = existing?.FirstClassCount,
                    DistinctionCount = existing?.DistinctionCount,
                    Remarks = existing?.Remarks
                });
            }

            model.AcademicRows = academicRows;


            // ============================================================
            // 2. COURSE CURRICULUM
            // ============================================================

            var curriculumMasters = await _context.CaMstCourseCurricula
                .AsNoTracking()
                .Where(x => x.IsActive != false)
                .OrderBy(x => x.CurriculumId)
                .ToListAsync();

            var savedCurriculums = await _context.CaCourseCurricula
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyId == facultyId &&
                    x.AffiliationType == affiliationType)
                .ToListAsync();

            model.CourseCurriculums = curriculumMasters
                .Select(master =>
                {
                    var saved = savedCurriculums
                        .FirstOrDefault(x =>
                            x.CurriculumId == master.CurriculumId);

                    return new CourseCurriculumViewModel
                    {
                        CourseCurriculumId =
                            saved?.CourseCurriculumId,

                        CurriculumId =
                            master.CurriculumId,

                        CurriculumName =
                            master.CurriculumName,

                        CurriculumDetails =
                            saved?.CurriculumDetails,

                        HasPdf =
                            saved != null &&
                            !string.IsNullOrWhiteSpace(
                                saved.CurriculumPdfPath)
                    };
                })
                .ToList();


            // ============================================================
            // 3. EXAMINATION SCHEMES
            // ============================================================

            var savedSchemes = await _context.CaExaminationSchemes
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyId == facultyId &&
                    x.AffiliationType == affiliationType)
                .ToListAsync();

            var schemeMasters = await _context.CaMstExaminationSchemes
                .AsNoTracking()
                .OrderBy(x => x.SchemeId)
                .ToListAsync();

            model.ExaminationSchemess = schemeMasters
                .Select(master =>
                {
                    var saved = savedSchemes
                        .FirstOrDefault(x =>
                            x.SchemeId == master.SchemeId);

                    return new ExaminationSchemeRowViewModel
                    {
                        SchemeId = master.SchemeId,
                        SchemeCode = master.SchemeCode,
                        NumberOfStudents =
                            saved?.NumberOfStudents
                    };
                })
                .ToList();


            // ============================================================
            // 4. STUDENT REGISTER RECORDS
            // ============================================================

            var registerMasters = await _context.CaMstRegisterRecords
                .AsNoTracking()
                .OrderBy(x => x.RegisterRecordId)
                .ToListAsync();

            var savedRegisters = await _context.CaStudentRegisterRecords
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyId == facultyId &&
                    x.AffiliationType == affiliationType)
                .ToListAsync();

            model.StudentRegisterRecords = registerMasters
                .Select(master =>
                {
                    var saved = savedRegisters
                        .FirstOrDefault(x =>
                            x.RegisterRecordId ==
                            master.RegisterRecordId);

                    return new StudentRegisterRecordViewModel
                    {
                        StudentRegisterRecordId =
                            saved?.StudentRegisterRecordId,

                        RegisterRecordId =
                            master.RegisterRecordId,

                        RegisterName =
                            master.RegisterName,

                        IsMaintained =
                            saved?.IsMaintained
                    };
                })
                .ToList();


            // ============================================================
            // 5. COMMON VIEW DATA
            // ============================================================

            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = institution.FacultyCode;
            ViewBag.InstitutionName =
                institution.NameOfInstitution ?? "Unknown Institution";

            ViewBag.ActiveTab = "UgAcademicMatters";
            ViewBag.UserDesignation = GetUserDesignation();

            // Existing verification information
            await SetVerificationViewData<CaAcademicPerformance>(
                collegeCode);


            return View("UgAcademicMatters", model);
        }

        [HttpGet]
        public async Task<IActionResult> PgAcademicMatters(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction(nameof(Index));

            // --------------------------------------------------
            // Common verification page context
            // --------------------------------------------------
            var pageContext = await GetPageContextAsync(collegeCode);

            var institution = pageContext.Institution;

            var facultyCode = institution.FacultyCode?.Trim();

            if (string.IsNullOrWhiteSpace(facultyCode))
                return NotFound("Faculty code not found.");

            if (!int.TryParse(facultyCode, out var facultyId))
                return NotFound("Invalid faculty code.");

            var affiliationType =
                HttpContext.Session.GetInt32("AffiliationType") ?? 2;

            const string courseLevel = "PG";

            // --------------------------------------------------
            // Subject Master
            // --------------------------------------------------
            var subjects = await (
                from c in _context.MstCourses
                join i in _context.CollegeCourseIntakeDetails
                    on c.CourseCode.ToString() equals i.CourseCode
                where c.CourseLevel == courseLevel
                      && i.CollegeCode == collegeCode
                      && i.FacultyCode == facultyId
                group c by new
                {
                    c.CourseCode,
                    c.SubjectName
                }
                into g
                orderby g.Key.SubjectName
                select new SelectListItem
                {
                    Value = g.Key.CourseCode.ToString(),
                    Text = g.Key.SubjectName
                }
            ).ToListAsync();

            // --------------------------------------------------
            // Fallback subject lookup
            // --------------------------------------------------
            if (!subjects.Any())
            {
                subjects = await (
                    from ai in _context.AcademicIntakes
                    join c in _context.MstCourses
                        on ai.Courses equals c.CourseCode.ToString()
                    where ai.CollegeCode == collegeCode
                          && c.CourseLevel == courseLevel
                    group c by new
                    {
                        c.CourseCode,
                        c.SubjectName
                    }
                    into g
                    orderby g.Key.SubjectName
                    select new SelectListItem
                    {
                        Value = g.Key.CourseCode.ToString(),
                        Text = g.Key.SubjectName
                    }
                ).ToListAsync();
            }

            var subjectLookup = subjects
                .ToDictionary(
                    x => x.Value,
                    x => x.Text
                );
            // --------------------------------------------------
            // Main ViewModel
            // --------------------------------------------------
            var model = new CA_Aff_PgAcademicMattersViewModel
            {
                CollegeCode = collegeCode,
                FacultyId = facultyId,
                AffiliationType = affiliationType,
                Subjects = subjects
            };

            // --------------------------------------------------
            // Year Master
            // --------------------------------------------------
            var yearMaster = await _context.CaMstYearOfStudies
                .AsNoTracking()
                .Take(3)
                .OrderBy(y => y.YearOfStudyId)
                .ToListAsync();

            ViewBag.YearList = yearMaster;

            // --------------------------------------------------
            // Existing PG Academic Performance
            // --------------------------------------------------
            var academics = await _context.CaAcademicPerformances
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyId == facultyId &&
                    x.AffiliationType == affiliationType &&
                    x.CourseLevel == courseLevel)
                .ToListAsync();

            // --------------------------------------------------
            // Group by Subject
            // --------------------------------------------------
            var sections = new List<PgSubjectSectionVM>();

            var grouped = academics
                .Where(x => !string.IsNullOrWhiteSpace(x.Subject))
                .GroupBy(x => x.Subject!);

            foreach (var subjectGroup in grouped)
            {
                var section = new PgSubjectSectionVM
                {
                    Subject = subjectLookup.TryGetValue(subjectGroup.Key, out var subjectName)
                        ? subjectName
                        : subjectGroup.Key,
                    YearData = new List<YearDataVM>()
                };

                foreach (var year in yearMaster)
                {
                    var existing = subjectGroup
                        .FirstOrDefault(x =>
                            x.YearOfStudyId == year.YearOfStudyId);

                    section.YearData.Add(new YearDataVM
                    {
                        YearOfStudyId = year.YearOfStudyId,
                        YearName = year.YearName,

                        RegularStudents =
                            existing?.RegularStudents,

                        RepeaterStudents =
                            existing?.RepeaterStudents,

                        NumberOfStudentsPassed =
                            existing?.NumberOfStudentsPassed,

                        PassPercentage =
                            existing?.PassPercentage,

                        FirstClassCount =
                            existing?.FirstClassCount,

                        DistinctionCount =
                            existing?.DistinctionCount,

                        Remarks =
                            existing?.Remarks
                    });
                }

                sections.Add(section);
            }

            // --------------------------------------------------
            // If no saved data exists, create empty sections
            // for the available PG subjects
            // --------------------------------------------------
            if (!sections.Any())
            {
                foreach (var subject in subjects)
                {
                    sections.Add(new PgSubjectSectionVM
                    {
                        Subject = subject.Text,
                        YearData = yearMaster
                            .Select(y => new YearDataVM
                            {
                                YearOfStudyId = y.YearOfStudyId,
                                YearName = y.YearName
                            })
                            .ToList()
                    });
                }
            }

            model.Sections = sections;

            // --------------------------------------------------
            // Verification View Data
            // --------------------------------------------------
            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = facultyCode;
            ViewBag.InstitutionName =
                institution.NameOfInstitution ?? "Unknown Institution";
            ViewBag.ActiveTab = "PgAcademicMatters";
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<CaAcademicPerformance>(
                collegeCode);

            return View("PgAcademicMatters", model);
        }

        [HttpGet]
        public async Task<IActionResult> LibraryServices( string collegeCode, string? courseLevel)
        {
            // ---------------------------------------------------------
            // COLLEGE CODE
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(collegeCode))
            {
                collegeCode = HttpContext.Session.GetString("CollegeCode");
            }

            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction("Login", "Account");


            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var pageContext = await GetPageContextAsync(collegeCode);

            var facultyCodeString =
                pageContext.Institution.FacultyCode?.Trim();

            if (string.IsNullOrWhiteSpace(facultyCodeString))
                return NotFound("Faculty code not found.");

            if (!int.TryParse(facultyCodeString, out var facultyCode))
                return NotFound("Invalid faculty code.");


            // ---------------------------------------------------------
            // AFFILIATION TYPE
            // ---------------------------------------------------------

            var affiliationType =
                HttpContext.Session.GetInt32("AffiliationType") ?? 2;


            // ---------------------------------------------------------
            // COURSE LEVELS
            // ---------------------------------------------------------

            var levels = await GetSortedCourseLevels(collegeCode);

            if (!levels.Any())
            {
                levels = new List<string> { "UG" };
            }

            levels = levels
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToUpper())
                .Distinct()
                .ToList();


            // ---------------------------------------------------------
            // SELECT COURSE LEVEL
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(courseLevel))
            {
                courseLevel =
                    levels.FirstOrDefault(x =>
                        x.Equals(
                            "UG",
                            StringComparison.OrdinalIgnoreCase))
                    ?? levels.First();
            }

            courseLevel = courseLevel.Trim().ToUpper();


            // Make sure requested level actually exists
            if (!levels.Contains(
                    courseLevel,
                    StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(
                    $"Course level '{courseLevel}' is not available.");
            }


            // ---------------------------------------------------------
            // VIEW MODEL
            // ---------------------------------------------------------

            var model = new MedicalLibraryVerificationViewModel
            {
                CollegeCode = collegeCode,
                FacultyCode = facultyCode,
                AffiliationType = affiliationType,
                CourseLevel = courseLevel
            };


            // =========================================================
            // 1. DEPARTMENT MASTER
            // =========================================================
            //
            // Used only to resolve DepartmentCode -> DepartmentName
            // for the verification UI.
            //
            // =========================================================

            var departmentMasters =
                await _context.DepartmentMasters
                    .AsNoTracking()
                    .Where(x =>
                        x.FacultyCode == facultyCode)
                    .OrderBy(x => x.DepartmentName)
                    .ToListAsync();


            // =========================================================
            // 2. DEPARTMENT LIBRARY
            // =========================================================

            var savedDepartments =
                await _context.CaMedicalDepartmentLibraries
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCode &&
                        x.AffiliationType == affiliationType)
                    .OrderBy(x => x.DepartmentalLibraryId)
                    .ToListAsync();


            model.DepartmentLibraries =
                savedDepartments
                    .Select(x =>
                    {
                        var staff1 = "";
                        var staff2 = "";

                        if (!string.IsNullOrWhiteSpace(x.LibraryStaff))
                        {
                            var parts =
                                x.LibraryStaff.Split(
                                    '|',
                                    StringSplitOptions.RemoveEmptyEntries);

                            if (parts.Length > 0)
                                staff1 = parts[0].Trim();

                            if (parts.Length > 1)
                                staff2 = parts[1].Trim();
                        }

                        return new DepartmentLibraryVerificationRow
                        {
                            DepartmentCode = x.DepartmentCode,

                            TotalBooks = x.TotalBooks,

                            BooksAddedInYear =
                                x.BooksAddedInYear,

                            CurrentJournals =
                                x.CurrentJournals,

                            LibraryStaff1 = staff1,

                            LibraryStaff2 = staff2,

                            Titles = x.Titles,

                            InternationalJournals =
                                x.InternationalJournals,

                            BackVolumes =
                                x.BackVolumes,

                            PrintJournalPercentage =
                                x.PrintJournalPercentage
                        };
                    })
                    .ToList();


            // =========================================================
            // 3. DENTAL LIBRARY RECORD MASTER
            // =========================================================

            if (facultyCode == 2)
            {
                var masterRecords =
                    await _context.CaMstDentalLibraryRecords
                        .AsNoTracking()
                        .OrderBy(x => x.DisplayOrder)
                        .ToListAsync();


                // =====================================================
                // 4. DENTAL LIBRARY UPLOADED RECORDS
                // =====================================================

                var uploadedRecords =
                    await _context.CaDentalLibraryRecords
                        .AsNoTracking()
                        .Where(x =>
                            x.CollegeCode == collegeCode &&
                            x.FacultyCode == facultyCode &&
                            x.AffiliationType == affiliationType)
                        .ToListAsync();


                model.DentalLibraryRecords =
                    masterRecords
                        .Select(master =>
                        {
                            var uploaded =
                                uploadedRecords.FirstOrDefault(x =>
                                    x.RecordId == master.RecordId);

                            return new DentalLibraryVerificationRow
                            {
                                RecordId = master.RecordId,

                                RecordName = master.RecordName,

                                ExistingFileName =
                                    uploaded?.FileName
                            };
                        })
                        .ToList();
            }


            // =========================================================
            // COMMON VIEW DATA
            // =========================================================

            ViewBag.InstitutionName =
                pageContext.Institution.NameOfInstitution;

            ViewBag.CollegeCode =
                collegeCode;

            ViewBag.FacultyCode =
                facultyCode;

            ViewBag.AffiliationType =
                affiliationType;

            ViewBag.CourseLevel =
                courseLevel;

            ViewBag.CourseLevels =
                levels;

            ViewBag.IsDentalFaculty =
                facultyCode == 2;

            ViewBag.DepartmentMasters =
                departmentMasters;


            // =========================================================
            // VERIFICATION DATA
            // =========================================================

            await SetVerificationViewData<CaMedicalDepartmentLibrary>(
                collegeCode);


            return View("LibraryServices", model);
        }


        [HttpGet]
        public async Task<IActionResult> ResearchPublications(string collegeCode)
        {
            // ---------------------------------------------------------
            // COLLEGE CODE
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(collegeCode))
            {
                collegeCode = HttpContext.Session.GetString("CollegeCode");
            }

            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction("Login", "Account");


            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var pageContext = await GetPageContextAsync(collegeCode);

            var facultyCodeString =
                pageContext.Institution.FacultyCode?.Trim();

            if (string.IsNullOrWhiteSpace(facultyCodeString))
                return NotFound("Faculty code not found.");

            if (!int.TryParse(facultyCodeString, out var facultyCode))
                return NotFound("Invalid faculty code.");


            // ---------------------------------------------------------
            // AFFILIATION TYPE
            // ---------------------------------------------------------

            var affiliationType =
                HttpContext.Session.GetInt32("AffiliationType") ?? 2;


            // =========================================================
            // 1. MAIN RESEARCH & PUBLICATIONS
            // =========================================================

            var researchRecord =
                await _context.CaMedResearchPublicationsDetails
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeString)
                    .OrderByDescending(x => x.SlNo)
                    .FirstOrDefaultAsync();


            // =========================================================
            // 2. DEPARTMENT-WISE PUBLICATIONS
            // =========================================================

            var departmentPublications =
                await _context.DeptWisePublications
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCode)
                    .OrderBy(x => x.DeptName)
                    .ToListAsync();


            // =========================================================
            // 3. BUILD VERIFICATION VIEW MODEL
            // =========================================================

            var model = new ResearchPublicationsVerificationViewModel
            {
                CollegeCode = collegeCode,

                FacultyCode = facultyCode,

                AffiliationType = affiliationType,

                // Main research details
                PublicationsNo =
                    researchRecord?.PublicationsNo,

                PublicationsPdfName =
                    researchRecord?.PublicationsPdfName,

                ClinicalTrialsPdfName =
                    researchRecord?.ClinicalTrialsPdfName,

                StudentsRGUHSFunded =
                    researchRecord?.StudentsRguhsfunded,

                StudentsExternalBodyFunding =
                    researchRecord?.StudentsExternalBodyFunding,

                StudentsProjectsPdfName =
                    researchRecord?.StudentsProjectsPdfName,

                FacultyRGUHSFunded =
                    researchRecord?.FacultyRguhsfunded,

                FacultyExternalBodyFunding =
                    researchRecord?.FacultyExternalBodyFunding,

                FacultyProjectsPdfName =
                    researchRecord?.FacultyProjectsPdfName,

                // Department-wise publications
                DepartmentWisePublications =
                    departmentPublications
                        .Select(x => new DepartmentWisePublicationVerificationRow
                        {
                            Id = x.Id,

                            DeptCode =
                                x.DeptCode,

                            DeptName =
                                x.DeptName,

                            PublicationsCount =
                                x.PublicationsCount,

                            PublicationPath =
                                x.PublicationPath
                        })
                        .ToList()
            };


            // =========================================================
            // 4. COMMON VIEW DATA
            // =========================================================

            ViewBag.InstitutionName =
                pageContext.Institution.NameOfInstitution;

            ViewBag.CollegeCode =
                collegeCode;

            ViewBag.FacultyCode =
                facultyCode;

            ViewBag.AffiliationType =
                affiliationType;


            // =========================================================
            // 5. VERIFICATION DATA
            // =========================================================

            await SetVerificationViewData<CaMedResearchPublicationsDetail>(
                collegeCode);


            return View("ResearchPublications", model);

        }


        [HttpGet] public async Task<IActionResult> ViewResearchPublicationPdf(string collegeCode) => await GetPdf(collegeCode, "Publications");
        [HttpGet] public async Task<IActionResult> ViewStudentsProjectsPdf(string collegeCode) => await GetPdf(collegeCode, "StudentsProjects");
        [HttpGet] public async Task<IActionResult> ViewFacultyProjectsPdf(string collegeCode) => await GetPdf(collegeCode, "FacultyProjects");
        [HttpGet] public async Task<IActionResult> ViewClinicalTrialsPdf(string collegeCode) => await GetPdf(collegeCode, "ClinicalTrials");

        private async Task<IActionResult> GetPdf(string collegeCode, string fileType)
        {

            // FIX: query "ALL" (matches how data is saved), fallback to any row
            var record = await _context.CaMedResearchPublicationsDetails
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode)
                ?? await _context.CaMedResearchPublicationsDetails
                    .FirstOrDefaultAsync(x =>
                        x.CollegeCode == collegeCode);

            if (record == null) return NotFound("Record not found.");

            string? filePath = fileType switch
            {
                "Publications" => record.PublicationsPdfPath,
                "StudentsProjects" => record.StudentsProjectsPdfPath,
                "FacultyProjects" => record.FacultyProjectsPdfPath,
                "ClinicalTrials" => record.ClinicalTrialsPdfPath,
                _ => null
            };

            string? name = fileType switch
            {
                "Publications" => record.PublicationsPdfName,
                "StudentsProjects" => record.StudentsProjectsPdfName,
                "FacultyProjects" => record.FacultyProjectsPdfName,
                "ClinicalTrials" => record.ClinicalTrialsPdfName,
                _ => null
            };

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return NotFound("File not found on server. Please re-upload.");

            var fileName = string.IsNullOrEmpty(name) ? Path.GetFileName(filePath) : name;

            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
                contentType = "application/octet-stream";

            Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
            return PhysicalFile(filePath, contentType);
        }


        [HttpGet]
        public async Task<IActionResult> ViewDepartmentPublicationPdf(string collegeCode, int id)
        {

            var publication = await _context.DeptWisePublications
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CollegeCode == collegeCode);

            if (publication == null ||
                string.IsNullOrWhiteSpace(publication.PublicationPath))
            {
                return NotFound("PDF not found.");
            }

            if (!System.IO.File.Exists(publication.PublicationPath))
            {
                return NotFound("File does not exist on server.");
            }

            var stream = new FileStream(
                publication.PublicationPath,
                FileMode.Open,
                FileAccess.Read);

            return File(stream, "application/pdf");
        }


        [HttpGet]
        public async Task<IActionResult> ViewDentalLibraryRecord(string collegeCode, int recordId)
        {

            int affiliationType =
                HttpContext.Session.GetInt32("AffiliationType") ?? 2;


            var record =
                await _context.CaDentalLibraryRecords
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.AffiliationType == affiliationType &&
                    x.RecordId == recordId);

            if (record == null ||
                string.IsNullOrEmpty(record.FilePath))
            {
                return NotFound();
            }

            if (!System.IO.File.Exists(record.FilePath))
            {
                return NotFound();
            }

            return PhysicalFile(
                record.FilePath,
                "application/pdf"
            );
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

                var institution = await _context.AffInstitutionsDetails.FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

                if (institution == null || string.IsNullOrWhiteSpace(institution.FacultyCode))
                {
                    TempData["ErrorMessage"] = "Faculty code not found.";
                    return RedirectToAction(tabName, new { collegeCode });
                }

                var facultyCode = Convert.ToInt32(institution.FacultyCode);

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

                    ["LandAndBuildingDetails"] = () =>
                        _verificationService.SaveVerificationAsync<DentalCollegeLandBuildingDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["ClassroomAndLaboratory"] = () =>
                        _verificationService.SaveVerificationAsync<DentalInfrastructure>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["ChairDistribution"] = () =>
                        _verificationService.SaveVerificationAsync<DentalChair>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["BedDistribution"] = () =>
                        _verificationService.SaveVerificationAsync<MedicalUgbedDistribution>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["HostelDetails"] = () =>
                        _verificationService.SaveVerificationAsync<AffHostelDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["DepartmentOfficesAndEducationalUnit"] = () =>
                        _verificationService.SaveVerificationAsync<MedicalDepartmentOfficesMeu>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["EquipmentList"] = () =>
                        _verificationService.SaveVerificationAsync<DentalCollegeEquipmentDetail>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode,
                            request),

                    ["CA_VehicleDetails"] = () =>
                        _verificationService.SaveVerificationAsync<CaVehicleDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["UgAcademicMatters"] = () =>
                        _verificationService.SaveVerificationAsync<CaAcademicPerformance>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["PgAcademicMatters"] = () =>
                        _verificationService.SaveVerificationAsync<CaAcademicPerformance>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["FinanceDetails"] = () =>
                        _verificationService.SaveVerificationAsync<MedCaAccountAndFeeDetail>(
                            x => x.CollegeCode == collegeCode,
                            request),

                    ["StaffPayScale"] = () =>
                        _verificationService.SaveVerificationAsync<MedCaStaffParticular>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request),

                    ["StaffOtherDetails"] = () =>
                        _verificationService.SaveVerificationAsync<CaMedStaffParticularsOther>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request),

                    ["LibraryServices"] = async () =>
                    {
                        // Department Library verification
                        await _verificationService.SaveVerificationAsync<CaMedicalDepartmentLibrary>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode,
                            request);

                        // Dental Library Records verification
                        await _verificationService.SaveVerificationAsync<CaDentalLibraryRecord>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode,
                            request);
                    },

                    ["ResearchPublications"] = async () =>
                    {
                        // Main Research & Publications details
                        await _verificationService.SaveVerificationAsync<CaMedResearchPublicationsDetail>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request);

                        // Department-wise Publications
                        await _verificationService.SaveVerificationAsync<DeptWisePublication>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode,
                            request);
                    },

                    ["LibraryDetails"] = async () =>
                    {
                        await _verificationService.SaveVerificationAsync<CaMedLibraryGeneral>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request);

                        await _verificationService.SaveVerificationAsync<CaMedLibraryItem>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request);

                        await _verificationService.SaveVerificationAsync<CaMedLibraryBuilding>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request);

                        await _verificationService.SaveVerificationAsync<CaMedLibTechnicalProcess>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request);

                        await _verificationService.SaveVerificationAsync<CaMedLibraryEquipment>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request);

                        await _verificationService.SaveVerificationAsync<CaMedLibraryFinance>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request);
                    },

                    ["FacultyDetails"] = () =>
                        _verificationService.SaveVerificationAsync<FacultyDetail>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
                            request),


                    ["TeachingExperience"] = async () =>
                    {
                        await _verificationService.SaveVerificationAsync<TeachingStaffDepartmentWiseDetail>(
                            x =>
                                x.CollegeCode == collegeCode &&
                                x.FacultyCode == facultyCode.ToString(),
                            request);
                    },

                    ["ClinicalFacilities"] = async() =>
                        await _verificationService.SaveVerificationAsync<HospitalDetailsForAffiliation>(
                            x => x.CollegeCode == collegeCode &&
                                 x.FacultyCode == facultyCode.ToString(),
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


        [HttpGet]
        public async Task<IActionResult> FinanceDetails(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction(nameof(Index));

            try
            {
                // Reusable page context
                var pageContext = await GetPageContextAsync(collegeCode);

                var institution = pageContext.Institution;
                var facultyCode = institution.FacultyCode!.Trim();

                // Get available course levels
                var levels = await (
                    from cc in _context.CollegeCourseIntakeDetails
                    join cm in _context.MstCourses
                        on cc.CourseCode equals cm.CourseCode.ToString()
                    where cc.CollegeCode == collegeCode
                    select cm.CourseLevel
                )
                .Distinct()
                .ToListAsync();

                // Fallback if no levels are found
                if (!levels.Any())
                {
                    levels = await GetSortedCourseLevels(collegeCode);
                }

                // Keep the desired order
                levels = levels
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim().ToUpper())
                    .Distinct()
                    .OrderBy(x => x == "UG" ? 1 :
                                  x == "PG" ? 2 :
                                  x == "SS" ? 3 : 99)
                    .ToList();

                // Load all saved finance records
                var financeRecords = await _context.MedCaAccountAndFeeDetails
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCode)
                    .ToListAsync();

                var vm = new Med_CA_AccountAndFeeDetailsPageVM();

                // Build one section for each course level
                foreach (var level in levels)
                {
                    var data = financeRecords.FirstOrDefault(x =>
                        !string.IsNullOrWhiteSpace(x.CourseLevel) &&
                        x.CourseLevel.Trim().Equals(level, StringComparison.OrdinalIgnoreCase));

                    vm.Sections.Add(new Med_CA_AccountAndFeeDetailsViewModel
                    {
                        CollegeCode = collegeCode,
                        FacultyCode = facultyCode,
                        CourseLevel = level,

                        AuthorityNameAddress = data?.AuthorityNameAddress ?? "",
                        AuthorityContact = data?.AuthorityContact ?? "",

                        RecurrentAnnual = data?.RecurrentAnnual,
                        NonRecurrentAnnual = data?.NonRecurrentAnnual,
                        Deposits = data?.Deposits,

                        TuitionFee = data?.TuitionFee,
                        SportsFee = data?.SportsFee,
                        UnionFee = data?.UnionFee,
                        LibraryFee = data?.LibraryFee,
                        OtherFee = data?.OtherFee,

                        TotalFee = data?.TotalFee ?? 0,

                        AccountBooksMaintained =
                            data?.AccountBooksMaintained ?? "",

                        AccountsAudited =
                            data?.AccountsAudited ?? "",

                        DonationLevied =
                            data?.DonationLevied ?? "",

                        GoverningCouncilPdfName =
                            data?.GoverningCouncilPdfName,

                        AccountSummaryPdfName =
                            data?.AccountSummaryPdfName,

                        AuditedStatementPdfName =
                            data?.AuditedStatementPdfName,

                        DonationPdfName =
                            data?.DonationPdfName
                    });
                }

                // Common verification page data
                ViewBag.CollegeCode = collegeCode;
                ViewBag.FacultyCode = facultyCode;
                ViewBag.InstitutionName =
                    institution.NameOfInstitution ?? "Unknown Institution";

                ViewBag.ActiveTab = "FinanceDetails";
                ViewBag.UserDesignation = GetUserDesignation();

                // Verification feedback
                await SetVerificationViewData<MedCaAccountAndFeeDetail>(collegeCode);

                return View("FinanceDetails", vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        // View PDF actions (keep these)
        [HttpGet]
        public async Task<IActionResult> ViewGoverningCouncilPdf(string courseLevel, string collegeCode, int facultyCode)
        {
            return await GetPdf("GoverningCouncil", courseLevel, collegeCode, facultyCode);
        }

        [HttpGet]
        public async Task<IActionResult> ViewAccountSummaryPdf(string courseLevel, string collegeCode, int facultyCode)
        {
            return await GetPdf("AccountSummary", courseLevel, collegeCode, facultyCode);
        }

        [HttpGet]
        public async Task<IActionResult> ViewAuditedStatementPdf(string courseLevel, string collegeCode, int facultyCode)
        {
            return await GetPdf("AuditedStatement", courseLevel, collegeCode, facultyCode);
        }

        [HttpGet]
        public async Task<IActionResult> ViewDonationPdf(string courseLevel, string collegeCode, int facultyCode)
        {
            return await GetPdf("Donation", courseLevel, collegeCode, facultyCode);
        }

        private async Task<IActionResult> GetPdf(string type, string courseLevel, string collegeCode, int facultyCode)
        {

            if (string.IsNullOrEmpty(courseLevel))
                return NotFound("Course level not specified.");

            var record = await _context.MedCaAccountAndFeeDetails
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode.ToString() &&
                    x.CourseLevel == courseLevel);

            if (record == null) return NotFound("Record not found.");

            string? filePath = type switch
            {
                "GoverningCouncil" => record.GoverningCouncilPdfPath,
                "AccountSummary" => record.AccountSummaryPdfPath,
                "AuditedStatement" => record.AuditedStatementPdfPath,
                "Donation" => record.DonationPdfPath,
                _ => null
            };

            string? name = type switch
            {
                "GoverningCouncil" => record.GoverningCouncilPdfName,
                "AccountSummary" => record.AccountSummaryPdfName,
                "AuditedStatement" => record.AuditedStatementPdfName,
                "Donation" => record.DonationPdfName,
                _ => null
            };

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return NotFound("File not found on server.");

            var fileName = string.IsNullOrEmpty(name) ? Path.GetFileName(filePath) : name;
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
                contentType = "application/octet-stream";

            Response.Headers["Content-Disposition"] = $"inline; filename=\"{fileName}\"";
            return PhysicalFile(filePath, contentType);
        }


        [HttpGet]
        public async Task<IActionResult> StaffPayScale(string collegeCode)
        {

            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction("Login", "Account");

            var pageContext = await GetPageContextAsync(collegeCode);

            var facultyCode = pageContext.Institution.FacultyCode?.Trim();

            if (string.IsNullOrWhiteSpace(facultyCode))
                return NotFound("Faculty code not found.");

            // Course levels
            var levels = await GetSortedCourseLevels(collegeCode);

            if (!levels.Any())
                levels = new List<string> { "UG" };

            // Staff designation master
            var designations = await _context.MedCaMstStaffDesignations
                .AsNoTracking()
                .Where(x => x.FacultyCode == facultyCode)
                .OrderBy(x => x.SlNo)
                .ToListAsync();

            // Saved pay scale records
            var savedPayScales = await _context.MedCaStaffParticulars
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode)
                .ToListAsync();

            // Prefer UG
            var selectedLevel = levels.Contains(
                "UG",
                StringComparer.OrdinalIgnoreCase)
                ? "UG"
                : levels.FirstOrDefault() ?? "UG";

            var payScaleList = designations
                .Select(d =>
                {
                    var saved = savedPayScales.FirstOrDefault(x =>
                        x.DesignationSlNo == d.SlNo &&
                        !string.IsNullOrWhiteSpace(x.CourseLevel) &&
                        x.CourseLevel.Trim()
                            .Equals(
                                selectedLevel,
                                StringComparison.OrdinalIgnoreCase));

                    return new Med_CA_StaffParticularsVM
                    {
                        DesignationSlNo = d.SlNo,
                        Designation = d.Designation,
                        PayScale = saved?.PayScale
                    };
                })
                .ToList();

            var vm = new StaffDetailsCombinedViewModel
            {
                CollegeCode = collegeCode,
                FacultyCode = facultyCode,
                StaffPayScaleList = payScaleList,
                ExistingCourseLevels = levels
            };

            // Verification data for THIS section only
            await SetVerificationViewData<MedCaStaffParticular>(
                collegeCode);

            ViewBag.InstitutionName =
                pageContext.Institution.NameOfInstitution;

            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = facultyCode;

            return View("StaffPayScale", vm);
        }

        [HttpGet]
        public async Task<IActionResult> StaffOtherDetails(string collegeCode)
        {

            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction("Login", "Account");

            var pageContext = await GetPageContextAsync(collegeCode);

            var facultyCode = pageContext.Institution.FacultyCode?.Trim();

            if (string.IsNullOrWhiteSpace(facultyCode))
                return NotFound("Faculty code not found.");

            // Course levels
            var levels = await GetSortedCourseLevels(collegeCode);

            if (!levels.Any())
                levels = new List<string> { "UG" };

            // Existing staff-other record
            var otherDetails = await _context.CaMedStaffParticularsOthers
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode);

            var staffOther = new CA_Med_StaffParticularsOtherVM();

            if (otherDetails != null)
            {
                staffOther.Id = otherDetails.Id;

                staffOther.CollegeCode = otherDetails.CollegeCode;
                staffOther.FacultyCode = otherDetails.FacultyCode;
                staffOther.RegistrationNo = otherDetails.RegistrationNo;
                staffOther.SubFacultyCode = otherDetails.SubFacultyCode;
                staffOther.CourseLevel = otherDetails.CourseLevel;

                staffOther.TeachersUpdatedInEMS =
                    otherDetails.TeachersUpdatedInEms;

                staffOther.ExaminerDetailsAttached =
                    otherDetails.ExaminerDetailsAttached;

                staffOther.ServiceRegisterMaintained =
                    otherDetails.ServiceRegisterMaintained;

                staffOther.AcquittanceRegisterMaintained =
                    otherDetails.AcquittanceRegisterMaintained;

                staffOther.ExaminerDetailsPdfName =
                    otherDetails.ExaminerDetailsPdfName;

                staffOther.ExaminerDetailsPdfName2 =
                    otherDetails.ExaminerDetailsPdfName2;

                staffOther.ExaminerDetailsPdfName3 =
                    otherDetails.ExaminerDetailsPdfName3;

                staffOther.ExaminerDetailsPdfName4 =
                    otherDetails.ExaminerDetailsPdfName4;

                staffOther.ExaminerDetailsPdfName5 =
                    otherDetails.ExaminerDetailsPdfName5;

                staffOther.AEBASLastThreeMonthsPdfName =
                    otherDetails.AebaslastThreeMonthsPdfName;

                staffOther.AEBASInspectionDayPdfName =
                    otherDetails.AebasinspectionDayPdfName;

                staffOther.ProvidentFundPdfName =
                    otherDetails.ProvidentFundPdfName;

                staffOther.ESIPdfName =
                    otherDetails.EsipdfName;
            }

            var vm = new StaffDetailsCombinedViewModel
            {
                CollegeCode = collegeCode,
                FacultyCode = facultyCode,
                StaffOther = staffOther,
                ExistingCourseLevels = levels
            };

            // Verification data for THIS section only
            await SetVerificationViewData<CaMedStaffParticularsOther>(
                collegeCode);

            ViewBag.InstitutionName =
                pageContext.Institution.NameOfInstitution;

            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = facultyCode;

            return View("StaffOtherDetails", vm);
        }


        [HttpGet]
        public async Task<IActionResult> ViewStaffOtherPdf(string collegeCode, string fileType)
        {

            if (string.IsNullOrEmpty(collegeCode))
                return NotFound();

            // Changes by Ram on 23/04/2026
            // Common records are loaded from UG master
            var courseLevel = "UG";

            var record = await _context.CaMedStaffParticularsOthers
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.CourseLevel == courseLevel);

            if (record == null)
                return NotFound();

            string? filePath = null;
            string? fileName = null;

            switch (fileType)
            {
                case "Examiner":
                    filePath = record.ExaminerDetailsPdfPath;
                    fileName = record.ExaminerDetailsPdfName;
                    break;

                case "Examiner2":
                    filePath = record.ExaminerDetailsPdfPath2;
                    fileName = record.ExaminerDetailsPdfName2;
                    break;

                case "Examiner3":
                    filePath = record.ExaminerDetailsPdfPath3;
                    fileName = record.ExaminerDetailsPdfName3;
                    break;

                case "Examiner4":
                    filePath = record.ExaminerDetailsPdfPath4;
                    fileName = record.ExaminerDetailsPdfName4;
                    break;

                case "Examiner5":
                    filePath = record.ExaminerDetailsPdfPath5;
                    fileName = record.ExaminerDetailsPdfName5;
                    break;

                case "AEBAS3Months":
                    filePath = record.AebaslastThreeMonthsPdfPath;
                    fileName = record.AebaslastThreeMonthsPdfName;
                    break;

                case "AEBASInspection":
                    filePath = record.AebasinspectionDayPdfPath;
                    fileName = record.AebasinspectionDayPdfName;
                    break;

                case "PF":
                    filePath = record.ProvidentFundPdfPath;
                    fileName = record.ProvidentFundPdfName;
                    break;

                case "ESI":
                    filePath = record.EsipdfPath;
                    fileName = record.EsipdfName;
                    break;

                default:
                    return NotFound();
            }

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return NotFound("File not found");

            var finalName =
                string.IsNullOrEmpty(fileName)
                ? Path.GetFileName(filePath)
                : fileName;

            var provider =
               new Microsoft.AspNetCore.StaticFiles
                  .FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(
                filePath,
                out string contentType))
            {
                contentType = "application/octet-stream";
            }

            //if (mode == "download")
            //{
            //    return PhysicalFile(
            //        filePath,
            //        contentType,
            //        finalName);
            //}

            Response.Headers["Content-Disposition"] =
              $"inline; filename=\"{finalName}\"";

            return PhysicalFile(filePath, contentType);
        }


        [HttpGet]
        public async Task<IActionResult> LibraryDetails(string collegeCode)
        {
            // ---------------------------------------------------------
            // COLLEGE CODE
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(collegeCode))
            {
                collegeCode = HttpContext.Session.GetString("CollegeCode");
            }

            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction("Login", "Account");


            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var pageContext =
                await GetPageContextAsync(collegeCode);

            var facultyCodeString =
                pageContext.Institution.FacultyCode?.Trim();

            if (string.IsNullOrWhiteSpace(facultyCodeString))
                return NotFound("Faculty code not found.");

            if (!int.TryParse(facultyCodeString, out var facultyCode))
                return NotFound("Invalid faculty code.");


            // ---------------------------------------------------------
            // AFFILIATION TYPE
            // ---------------------------------------------------------

            var affiliationType =
                HttpContext.Session.GetInt32("AffiliationType") ?? 2;


            // =========================================================
            // 1. GENERAL LIBRARY DETAILS
            // =========================================================

            var general =
                await _context.CaMedLibraryGenerals
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeString)
                    .OrderByDescending(x => x.SlNo)
                    .FirstOrDefaultAsync();


            // =========================================================
            // 2. LIBRARY ITEMS
            // =========================================================

            var itemRows =
                await _context.CaMedLibraryItems
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeString)
                    .OrderBy(x => x.SlNo)
                    .ToListAsync();

            var items = itemRows
                .GroupBy(x => x.SlNo)
                .Select(g => g.First())
                .OrderBy(x => x.SlNo)
                .ToList();

            // =========================================================
            // 3. LIBRARY BUILDING
            // =========================================================

            var building =
                await _context.CaMedLibraryBuildings
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeString)
                    .OrderByDescending(x => x.SlNo)
                    .FirstOrDefaultAsync();


            // =========================================================
            // 4. TECHNICAL PROCESSES
            // =========================================================

            var technicalProcessRows =
                await _context.CaMedLibTechnicalProcesses
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeString)
                    .OrderBy(x => x.SlNo)
                    .ToListAsync();

            var technicalProcesses = technicalProcessRows
                .GroupBy(x => x.SlNo)
                .Select(g => g.First())
                .OrderBy(x => x.SlNo)
                .ToList();

            // =========================================================
            // 5. EQUIPMENT
            // =========================================================

            var equipmentRows =
                await _context.CaMedLibraryEquipments
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeString)
                    .OrderBy(x => x.SlNo)
                    .ToListAsync();

            var equipments = equipmentRows
                .GroupBy(x => x.SlNo)
                .Select(g => g.First())
                .OrderBy(x => x.SlNo)
                .ToList();


            // =========================================================
            // 6. FINANCE
            // =========================================================

            var finance =
                await _context.CaMedLibraryFinances
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeString)
                    .OrderBy(x => x.SlNo)
                    .FirstOrDefaultAsync();


            // =========================================================
            // VIEW MODEL
            // =========================================================

            var model =
                new LibraryDetailsVerificationViewModel
                {
                    CollegeCode = collegeCode,

                    FacultyCode = facultyCode,

                    AffiliationType = affiliationType,

                    General = general == null
                        ? null
                        : new LibraryGeneralVerification
                        {
                            LibraryEmailId =
                                general.LibraryEmailId,

                            DigitalLibrary =
                                general.DigitalLibrary,

                            HelinetServices =
                                general.HelinetServices,

                            DepartmentWiseLibrary =
                                general.DepartmentWiseLibrary
                        },

                    Items = items
                        .Select(x => new LibraryItemVerificationRow
                        {
                            SlNo = x.SlNo,

                            ItemName = x.ItemName,

                            CurrentForeign =
                                x.CurrentForeign,

                            CurrentIndian =
                                x.CurrentIndian,

                            PreviousForeign =
                                x.PreviousForeign,

                            PreviousIndian =
                                x.PreviousIndian
                        })
                        .ToList(),

                    Building = building == null
                        ? null
                        : new LibraryBuildingVerification
                        {
                            IsIndependent =
                                building.IsIndependent,

                            AreaSqMtrs =
                                building.AreaSqMtrs
                        },

                    TechnicalProcesses = technicalProcesses
                        .Select(x => new LibraryTechnicalProcessVerificationRow
                        {
                            SlNo = x.SlNo,

                            ProcessName = x.ProcessName,

                            Value = x.Value
                        })
                        .ToList(),

                    Equipments = equipments
                        .Select(x => new LibraryEquipmentVerificationRow
                        {
                            SlNo = x.SlNo,

                            EquipmentName = x.EquipmentName,

                            HasEquipment = x.HasEquipment
                        })
                        .ToList(),

                    Finance = finance == null
                        ? null
                        : new LibraryFinanceVerification
                        {
                            TotalBudgetLakhs =
                                finance.TotalBudgetLakhs,

                            ExpenditureBooksLakhs =
                                finance.ExpenditureBooksLakhs
                        }

                };


            // ---------------------------------------------------------
            // COMMON VIEW DATA
            // ---------------------------------------------------------

            ViewBag.InstitutionName =
                pageContext.Institution.NameOfInstitution;

            ViewBag.CollegeCode =
                collegeCode;

            ViewBag.FacultyCode =
                facultyCode;

            ViewBag.AffiliationType =
                affiliationType;


            // ---------------------------------------------------------
            // VERIFICATION DATA
            // ---------------------------------------------------------

            await SetVerificationViewData<CaMedLibraryGeneral>(
                collegeCode);


            return View("LibraryDetails", model);
        }

        [HttpGet]
        public async Task<IActionResult> FacultyDetails(string collegeCode)
        {
            // ---------------------------------------------------------
            // COLLEGE CODE
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(collegeCode))
            {
                collegeCode =
                    HttpContext.Session.GetString("CollegeCode");
            }

            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction("Login", "Account");


            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var pageContext =
                await GetPageContextAsync(collegeCode);

            var facultyCodeString =
                pageContext.Institution.FacultyCode?.Trim();

            if (string.IsNullOrWhiteSpace(facultyCodeString))
                return NotFound("Faculty code not found.");

            if (!int.TryParse(facultyCodeString, out var facultyCode))
                return NotFound("Invalid faculty code.");


            // ---------------------------------------------------------
            // AFFILIATION TYPE
            // ---------------------------------------------------------

            var affiliationType =
                HttpContext.Session.GetInt32("AffiliationType") ?? 2;


            // =========================================================
            // FACULTY DETAILS
            // =========================================================

            var facultyDetails =
                await _context.FacultyDetails
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeString &&
                        x.IsRemoved != true)
                    .OrderBy(x => x.NameOfFaculty)
                    .ToListAsync();

            // =========================================================
            // 2. DESIGNATION MASTER
            // =========================================================

            var designationMasters =
                await _context.DesignationMasters
                    .AsNoTracking()
                    .Where(x => x.FacultyCode == facultyCode)
                    .ToListAsync();

            var designationDictionary =
                designationMasters
                    .GroupBy(x => x.DesignationCode)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().DesignationName);


            // =========================================================
            // 3. DEPARTMENT / SUBJECT MASTER
            // =========================================================

            var courseMasters =
                await _context.MstCourses
                    .AsNoTracking()
                    .Where(x =>
                        x.FacultyCode.ToString() == facultyCodeString &&
                        x.SubjectName != null)
                    .ToListAsync();

            var departmentDictionary =
                courseMasters
                    .GroupBy(x => new
                    {
                        x.SubjectName,
                        x.CourseLevel
                    })
                    .ToDictionary(
                        g => g.First().CourseCode.ToString(),
                        g => g.Key.SubjectName!);

            // =========================================================
            // VIEW MODEL
            // =========================================================

            var model =
                new FacultyDetailsVerificationViewModel
                {
                    CollegeCode = collegeCode,

                    FacultyCode = facultyCode,

                    AffiliationType = affiliationType,

                    FacultyDetails =
                        facultyDetails
                            .Select(x =>
                            {
                                designationDictionary.TryGetValue(
                                    x.Designation ?? "",
                                    out var designationName);

                                departmentDictionary.TryGetValue(
                                    x.DepartmentDetails ?? "",
                                    out var departmentName);

                                return new FacultyDetailsVerificationRow
                                {
                                    Id = x.Id,

                                    NameOfFaculty =
                                        x.NameOfFaculty,

                                    Designation =
                                        designationName
                                        ?? x.Designation,

                                    DepartmentDetails =
                                        departmentName
                                        ?? x.DepartmentDetails,

                                    RecognizedPgTeacher =
                                        x.RecognizedPgTeacher,

                                    Mobile =
                                        x.Mobile,

                                    Email =
                                        x.Email,

                                    Pan =
                                        x.Pan,

                                    Aadhaar =
                                        x.Aadhaar,

                                    RecognizedPhDteacher =
                                        x.RecognizedPhDteacher,

                                    LitigationPending =
                                        x.LitigationPending,

                                    IsExaminer =
                                        x.IsExaminer,

                                    ExaminerFor =
                                        x.ExaminerFor,

                                    GuideRecognitionDocPath =
                                        x.GuideRecognitionDocPath,

                                    PhDrecognitionDocPath =
                                        x.PhDrecognitionDocPath,

                                    LitigationDocPath =
                                        x.LitigationDocPath,

                                    From =
                                        x.From,

                                    To =
                                        x.To,

                                    RemoveRemarks =
                                        x.RemoveRemarks
                                };
                            })
                            .ToList()
                };


            // ---------------------------------------------------------
            // COMMON VIEW DATA
            // ---------------------------------------------------------

            ViewBag.InstitutionName =
                pageContext.Institution.NameOfInstitution;

            ViewBag.CollegeCode =
                collegeCode;

            ViewBag.FacultyCode =
                facultyCode;

            ViewBag.AffiliationType =
                affiliationType;


            // ---------------------------------------------------------
            // VERIFICATION DATA
            // ---------------------------------------------------------

            await SetVerificationViewData<FacultyDetail>(
                collegeCode);


            return View("FacultyDetails", model);
        }


        [HttpGet]
        public async Task<IActionResult> TeachingExperience(string collegeCode)
        {
            // ---------------------------------------------------------
            // COLLEGE CODE
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(collegeCode))
            {
                collegeCode =
                    HttpContext.Session.GetString("CollegeCode");
            }

            if (string.IsNullOrWhiteSpace(collegeCode))
                return RedirectToAction("Login", "Account");


            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var pageContext =
                await GetPageContextAsync(collegeCode);

            var facultyCodeString =
                pageContext.Institution.FacultyCode?.Trim();

            if (string.IsNullOrWhiteSpace(facultyCodeString))
                return NotFound("Faculty code not found.");

            if (!int.TryParse(facultyCodeString, out var facultyCode))
                return NotFound("Invalid faculty code.");


            // ---------------------------------------------------------
            // AFFILIATION TYPE
            // ---------------------------------------------------------

            var affiliationType =
                HttpContext.Session.GetInt32("AffiliationType") ?? 2;


            // =========================================================
            // 1. TEACHING EXPERIENCE
            // =========================================================

            var teachingRecords =
                await _context.TeachingStaffDepartmentWiseDetails
                    .AsNoTracking()
                    .Where(x =>
                        x.CollegeCode == collegeCode &&
                        x.FacultyCode == facultyCodeString &&
                        !string.IsNullOrWhiteSpace(x.NameOfFaculty))
                    .OrderBy(x => x.NameOfFaculty)
                    .ThenBy(x => x.DepartmentCode)
                    .ThenBy(x => x.CourseLevel)
                    .ThenBy(x => x.DesignationName)
                    .ToListAsync();

            var collegeMasters =
                await _context.AffiliationCollegeMasters
                    .AsNoTracking()
                    .Where(x => x.FacultyCode == facultyCodeString)
                    .ToListAsync();

            var otherCollegeMasters =
                await _context.AffiliationOthersCollegeMasters
                    .AsNoTracking()
                    .Where(x => x.FacultyCode == facultyCode)
                    .ToListAsync();

            var collegeDictionary =
                collegeMasters
                    .Where(x => !string.IsNullOrWhiteSpace(x.CollegeCode))
                    .Select(x => new
                    {
                        Code = x.CollegeCode,
                        Name = x.CollegeName ?? x.CollegeCode
                    })
                    .Concat(
                        otherCollegeMasters
                            .Where(x => !string.IsNullOrWhiteSpace(x.CollegeCode))
                            .Select(x => new
                            {
                                Code = x.CollegeCode,
                                Name = x.CollegeName ?? x.CollegeCode
                            })
                    )
                    .GroupBy(x => x.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().Name,
                        StringComparer.OrdinalIgnoreCase);


            // =========================================================
            // 2. DEPARTMENT MASTER
            // =========================================================

            var courseMasters =
                await _context.MstCourses
                    .AsNoTracking()
                    .Where(x =>
                        x.FacultyCode == facultyCode)
                    .ToListAsync();

            var departmentDictionary =
                courseMasters
                    .Where(x => x.CourseCode != null)
                    .GroupBy(x => x.CourseCode.ToString())
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().CourseName);


            // =========================================================
            // 3. DESIGNATION MASTER
            // =========================================================

            var designationMasters =
                await _context.DesignationMasters
                    .AsNoTracking()
                    .Where(x =>
                        x.FacultyCode == facultyCode)
                    .ToListAsync();

            var designationDictionary =
                designationMasters
                    .GroupBy(x => x.DesignationCode)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().DesignationName);


            // =========================================================
            // 4. VIEW MODEL
            // =========================================================

            var model =
                new TeachingExperienceVerificationViewModel
                {
                    CollegeCode = collegeCode,

                    FacultyCode = facultyCode,

                    AffiliationType = affiliationType,

                    FacultyRows =
                        teachingRecords
                            .GroupBy(x => x.NameOfFaculty!.Trim())
                            .OrderBy(g => g.Key)
                            .Select(facultyGroup =>
                            {
                                var facultyVm =
                                    new TeachingExperienceFacultyRow
                                    {
                                        NameOfFaculty =
                                            facultyGroup.Key
                                    };


                                // -----------------------------------------
                                // GROUP BY DEPARTMENT + COURSE LEVEL
                                // -----------------------------------------

                                facultyVm.Departments =
                                    facultyGroup
                                        .GroupBy(x => new
                                        {
                                            x.DepartmentCode,
                                            x.CourseLevel
                                        })
                                        .OrderBy(g => g.Key.DepartmentCode)
                                        .ThenBy(g => g.Key.CourseLevel)
                                        .Select(departmentGroup =>
                                        {
                                            departmentDictionary.TryGetValue(
                                                departmentGroup.Key.DepartmentCode ?? "",
                                                out var departmentName);

                                            var departmentVm =
                                                new TeachingExperienceDepartmentRow
                                                {
                                                    DepartmentCode =
                                                        departmentGroup.Key.DepartmentCode,

                                                    DepartmentName =
                                                        departmentName
                                                        ?? departmentGroup.Key.DepartmentCode,

                                                    CourseLevel =
                                                        departmentGroup.Key.CourseLevel
                                                };


                                            // ---------------------------------
                                            // EXPERIENCE RECORDS
                                            // ---------------------------------

                                            departmentVm.Experiences =
                                                departmentGroup
                                                    .Select(x =>
                                                    {
                                                        designationDictionary.TryGetValue(
                                                            x.DesignationCode ?? "",
                                                            out var designationName);

                                                        var experience =
                                                            new TeachingExperienceDetailRow
                                                            {
                                                                Id = x.Id,

                                                                DesignationCode =
                                                                    x.DesignationCode,

                                                                DesignationName =
                                                                    designationName
                                                                    ?? x.DesignationName,

                                                                CourseLevel =
                                                                    x.CourseLevel,

                                                                UgFrom =
                                                                    x.Ugfrom,

                                                                UgTo =
                                                                    x.Ugto,

                                                                PgFrom =
                                                                    x.Pgfrom,

                                                                PgTo =
                                                                    x.Pgto,

                                                                // -----------------------------------------
                                                                // UG COLLEGE
                                                                // -----------------------------------------

                                                                UgCollegeCode =
                                                                    x.UgcollegeCode,

                                                                UgCollegeName =
                                                                    !string.IsNullOrWhiteSpace(x.UgcollegeCode) &&
                                                                    collegeDictionary.TryGetValue(
                                                                        x.UgcollegeCode,
                                                                        out var ugCollegeName)
                                                                        ? ugCollegeName
                                                                        : x.UgcollegeCode,

                                                                // -----------------------------------------
                                                                // PG COLLEGE
                                                                // -----------------------------------------

                                                                PgCollegeCode =
                                                                    x.PgcollegeCode,

                                                                PgCollegeName =
                                                                    !string.IsNullOrWhiteSpace(x.PgcollegeCode) &&
                                                                    collegeDictionary.TryGetValue(
                                                                        x.PgcollegeCode,
                                                                        out var pgCollegeName)
                                                                        ? pgCollegeName
                                                                        : x.PgcollegeCode,

                                                                TotalExperience =
                                                                    x.TotalExperience,

                                                                FacultyDetailId =
                                                                    x.FacultyDetailId
                                                            };

                                                        return experience;
                                                    })
                                                    .ToList();

                                            return departmentVm;
                                        })
                                        .ToList();


                                // -----------------------------------------
                                // TOTAL EXPERIENCE
                                // -----------------------------------------

                                var dates =
                                    facultyGroup
                                        .SelectMany(x =>
                                            new[]
                                            {
                                        x.Ugfrom,
                                        x.Pgfrom
                                            })
                                        .Where(x => x.HasValue)
                                        .Select(x =>
                                            x!.Value.ToDateTime(
                                                TimeOnly.MinValue))
                                        .ToList();

                                if (dates.Any())
                                {
                                    var fromDate = dates.Min();

                                    var toDates =
                                        facultyGroup
                                            .SelectMany(x =>
                                                new[]
                                                {
                                            x.Ugto,
                                            x.Pgto
                                                })
                                            .Where(x => x.HasValue)
                                            .Select(x =>
                                                x!.Value.ToDateTime(
                                                    TimeOnly.MinValue))
                                            .ToList();

                                    var toDate =
                                        toDates.Any()
                                            ? toDates.Max()
                                            : DateTime.Today;

                                    facultyVm.TotalExperience =
                                        CalculateExperience(
                                            fromDate,
                                            toDate);
                                }

                                return facultyVm;
                            })
                            .ToList()
                };


            // ---------------------------------------------------------
            // COMMON VIEW DATA
            // ---------------------------------------------------------

            ViewBag.InstitutionName =
                pageContext.Institution.NameOfInstitution;

            ViewBag.CollegeCode =
                collegeCode;

            ViewBag.FacultyCode =
                facultyCode;

            ViewBag.AffiliationType =
                affiliationType;


            // ---------------------------------------------------------
            // VERIFICATION DATA
            // ---------------------------------------------------------

            await SetVerificationViewData<TeachingStaffDepartmentWiseDetail>(
                collegeCode);


            return View("TeachingExperience", model);
        }

        private decimal CalculateExperience(DateTime from, DateTime to)
        {
            int years = to.Year - from.Year;
            int months = to.Month - from.Month;

            if (to.Day < from.Day)
                months--;

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return years + (months / 12m);
        }


        [HttpGet]
        public async Task<IActionResult> ClinicalFacilities(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            var context = await GetPageContextAsync(collegeCode);

            // Same common setup as HostelDetails
            PopulateCommonViewBags(context);

            ViewBag.ActiveTab = "ClinicalFacilities";
            ViewBag.UserDesignation = GetUserDesignation();

            // Build Clinical Facilities model
            var model =
                await _clinicalFacilitiesCompositeService
                    .GetClinicalFacilitiesAsync(
                        collegeCode,
                        context);

            // IMPORTANT:
            // This loads existing verification status / remarks
            // and sets the ViewData required by the feedback form.
            await SetVerificationViewData<HospitalDetailsForAffiliation>(
                collegeCode);

            return View(model);
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