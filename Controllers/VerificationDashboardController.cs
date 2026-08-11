using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
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

                { "ClassroomAndLaboratory", typeof(DentalInfrastructure) },
                { "TeachingStaffDepartmentWise", typeof(TeachingStaffDepartmentWiseDetail) },
                { "AcademicIntake", typeof(CollegeCourseIntakeDetail) }
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