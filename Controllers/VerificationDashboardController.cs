using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Models.ViewModels;
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
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var context = await GetPageContextAsync(collegeCode);

            PopulateCommonViewBags(context);

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;

            ViewBag.ActiveTab = "InstitutionDetails";
            ViewBag.UserDesignation = GetUserDesignation();


            // ---------------------------------------------------------
            // COLLEGE
            // ---------------------------------------------------------

            var college = await _context.AffiliationCollegeMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.CollegeCode == collegeCode);

            ViewBag.CollegeName =
                college?.CollegeName ?? "Unknown College";


            // ---------------------------------------------------------
            // INSTITUTION DETAILS
            // ---------------------------------------------------------

            var institution = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode);

            if (institution == null)
            {
                return NotFound(
                    $"Institution details not found for college code: {collegeCode}");
            }


            // ---------------------------------------------------------
            // TYPE OF INSTITUTION
            // ---------------------------------------------------------

            string typeOfInstitution = institution.TypeOfInstitution;

            if (int.TryParse(institution.TypeOfInstitution, out int institutionTypeId))
            {
                typeOfInstitution =
                    await _context.MstInstitutionTypes
                        .AsNoTracking()
                        .Where(x =>
                            x.InstitutionTypeId == institutionTypeId)
                        .Select(x => x.InstitutionType)
                        .FirstOrDefaultAsync()
                    ?? institution.TypeOfInstitution;
            }

            ViewBag.TypeOfInstitution = typeOfInstitution;


            // ---------------------------------------------------------
            // STATUS OF COLLEGE
            // ---------------------------------------------------------

            string statusOfCollege = institution.StatusOfCollege;

            if (byte.TryParse(institution.StatusOfCollege, out byte statusId))
            {
                statusOfCollege =
                    await _context.AffInstitutionStatusMasters
                        .AsNoTracking()
                        .Where(x =>
                            x.InstitutionStatusId == statusId &&
                            x.IsActive)
                        .Select(x => x.StatusName)
                        .FirstOrDefaultAsync()
                    ?? institution.StatusOfCollege;
            }

            ViewBag.StatusOfCollege = statusOfCollege;


            // ---------------------------------------------------------
            // TALUK
            // ---------------------------------------------------------

            string taluk = institution.Taluk;

            if (!string.IsNullOrWhiteSpace(institution.Taluk))
            {
                taluk =
                    await _context.TalukMasters
                        .AsNoTracking()
                        .Where(x =>
                            x.TalukId == institution.Taluk)
                        .Select(x => x.TalukName)
                        .FirstOrDefaultAsync()
                    ?? institution.Taluk;
            }

            ViewBag.Taluk = taluk;


            // ---------------------------------------------------------
            // DISTRICT
            // ---------------------------------------------------------

            string district = institution.District;

            if (!string.IsNullOrWhiteSpace(institution.District))
            {
                district =
                    await _context.DistrictMasters
                        .AsNoTracking()
                        .Where(x =>
                            x.DistrictId == institution.District)
                        .Select(x => x.DistrictName)
                        .FirstOrDefaultAsync()
                    ?? institution.District;
            }

            ViewBag.District = district;


            // ---------------------------------------------------------
            // VERIFICATION
            // ---------------------------------------------------------

            await SetVerificationViewData<AffInstitutionsDetail>(
                collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync( collegeCode, 1);

            // ---------------------------------------------------------
            // VIEW
            // ---------------------------------------------------------

            return View(institution);
        }

        private async Task<List<SectionFeedbackViewModel>> GetTabSectionFeedbackAsync(string collegeCode, int tabId)
        {
            // Admin users don't need section-wise feedback - they only use page-wise verification
            if (IsAdminUser())
                return new List<SectionFeedbackViewModel>();

            var sections = await _context.MstSections
                .AsNoTracking()
                .Where(x => x.TabId == tabId)
                .OrderBy(x => x.SectionId)
                .ToListAsync();

            var feedback = await _context.SectionWiseFeedbacks
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.TabId == tabId)
                .ToListAsync();

            var returnUrl = $"{Request.Path}{Request.QueryString}";

            return sections.Select(section =>
            {
                var sectionFeedback = feedback.FirstOrDefault(x =>
                    x.SectionId == section.SectionId);

                return new SectionFeedbackViewModel
                {
                    FacultyId = sectionFeedback?.FacultyId ?? 2,
                    CollegeCode = collegeCode,
                    TabId = tabId,
                    SectionId = section.SectionId,
                    SectionName = section.SectionName,

                    VerificationStatus = sectionFeedback?.VerificationStatus,
                    Remarks = sectionFeedback?.Remarks,
                    VerifiedBy = sectionFeedback?.VerifiedBy,
                    VerifiedOn = sectionFeedback?.VerifiedOn,
                    IsSaved = sectionFeedback != null,
                    ReturnUrl = returnUrl
                };
            }).ToList();
        }

        // GET: /VerificationDashboard/TrustDetails/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> TrustDetails(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            var context =
                await GetPageContextAsync(collegeCode);

            PopulateCommonViewBags(context);

            ViewBag.ActiveTab =  ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();

            var institution =
                await _context.InstitutionBasicDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i =>
                        i.CollegeCode == collegeCode);

            if (institution == null)
            {
                return NotFound(
                    $"Institution details not found for college code: {collegeCode}");
            }

            var college =
                await _context.AffiliationCollegeMasters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c =>
                        c.CollegeCode == collegeCode);

            ViewBag.CollegeName =
                college?.CollegeName ?? "Unknown College";

            // ---------------------------------------------------------
            // VERIFICATION
            // ---------------------------------------------------------

            VerificationDisplayModel? verification = null;

            bool hasSectionData = institution != null;

            try
            {
                verification = await _verificationService
                    .GetVerificationAsync<AffInstitutionsDetail>(
                        x => x.CollegeCode == collegeCode,
                        GetUserDesignation());
            }
            catch (Exception ex) when (
                ex.Message.Contains("record not found"))
            {
                verification = null;
            }

            SetVerificationViewData(
                verification,
                hasSectionData,
                "Institution details have not been submitted for this college.");

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(
                collegeCode,
                1);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 2);

            return View(institution);
        }

        public IActionResult ViewTrustDetailsPageDocument(int id, string documentType)
        {
            var institution = _context.InstitutionBasicDetails
                .FirstOrDefault(x => x.InstitutionId == id);

            if (institution == null)
                return NotFound();

            string? storedPath = documentType switch
            {
                "AmendedDoc" =>
                    institution.GokOrderExistingCoursesFilePath,

                "Panfile" =>
                    institution.PanfilePath,

                "BankStatement" =>
                    institution.BankStatementFilePath,

                "RegistrationCertificate" =>
                    institution.RegistrationCertificateFilePath,

                "AuditStatement" =>
                    institution.AuditStatementFilePath,

                _ => null
            };

            if (string.IsNullOrWhiteSpace(storedPath))
                return NotFound();

            var filePath = ResolveDocumentPath(storedPath);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(
                filePath,
                GetDocumentContentType(filePath));
        }

        private string ResolveDocumentPath(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return string.Empty;

            filePath = filePath.Replace('/', '\\');

            // Existing absolute path
            if (Path.IsPathRooted(filePath) &&
                System.IO.File.Exists(filePath))
            {
                return filePath;
            }

            // Remove the stored BaseDentalPath if it is already included
            var basePath = BaseDentalPath.TrimEnd('\\');

            if (filePath.StartsWith(
                basePath,
                StringComparison.OrdinalIgnoreCase))
            {
                filePath = filePath.Substring(basePath.Length)
                                   .TrimStart('\\');
            }

            return Path.Combine(BaseDentalPath, filePath);
        }

        private string GetDocumentContentType(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
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
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();
            ViewBag.InstitutionName = institution?.NameOfInstitution ?? "Unknown Institution";

            var trustMemberDocument =
                await _context.ContinuationTrustMemberDocuments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.CollegeCode == collegeCode);


            ViewBag.RegisteredTrustMemberDetailsPath = trustMemberDocument?.RegisteredTrustMemberDetailsPath;


            // ---------------------------------------------------------
            // VERIFICATION
            // ---------------------------------------------------------

            VerificationDisplayModel? verification = null;

            bool hasSectionData = institution != null;

            try
            {
                verification = await _verificationService
                    .GetVerificationAsync<AffInstitutionsDetail>(
                        x => x.CollegeCode == collegeCode,
                        GetUserDesignation());
            }
            catch (Exception ex) when (
                ex.Message.Contains("record not found"))
            {
                verification = null;
            }

            SetVerificationViewData( verification, hasSectionData, "Institution details have not been submitted for this college.");


            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 3);

            return View(trustMembers);
        }


        [HttpGet]
        public async Task<IActionResult> ViewTrustMemberDocument(string collegeCode, string documentType)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound();

            if (documentType != "RegisteredTrustMemberDetails")
                return NotFound();

            var document =
                await _context.ContinuationTrustMemberDocuments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.CollegeCode == collegeCode);

            if (document == null)
                return NotFound();

            var filePath = document.RegisteredTrustMemberDetailsPath;

            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound();

            var resolvedPath = ResolveDocumentPath(filePath);

            if (!System.IO.File.Exists(resolvedPath))
                return NotFound();

            return PhysicalFile(
                resolvedPath,
                GetDocumentContentType(resolvedPath));
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


        private async Task<List<SelectListItem>> GetAffiliationCollegesAsync( string facultyCode)
        {
            var colleges = await _context.AffiliationCollegeMasters
                .AsNoTracking()
                .Where(e =>
                    e.FacultyCode.Trim() == facultyCode.Trim())
                .Select(e => new SelectListItem
                {
                    Value = e.CollegeCode,
                    Text = e.CollegeName + ", " + e.CollegeTown
                })
                .ToListAsync();

            return colleges;
        }


        private async Task<List<SelectListItem>> GetAffiliationOtherCollegesAsync(string facultyCode)
        {
            var otherColleges = await _context.AffiliationOthersCollegeMasters
                .AsNoTracking()
                .Where(e =>
                    e.FacultyCode.ToString() == facultyCode.Trim())
                .Select(e => new SelectListItem
                {
                    Value = e.CollegeCode,
                    Text = e.CollegeName + ", " + e.CollegeTown
                })
                .ToListAsync();

            return otherColleges;
        }


        private async Task<List<SelectListItem>> GetAllExperienceCollegesAsync( string facultyCode )
        {
            var colleges =
                await GetAffiliationCollegesAsync(facultyCode);

            var otherColleges =
                await GetAffiliationOtherCollegesAsync(facultyCode);

            return colleges
                .Concat(otherColleges)
                .GroupBy(x => x.Value)
                .Select(g => g.First())
                .OrderBy(x => x.Text)
                .ToList();
        }

        // GET: /VerificationDashboard/DeanDirectorDetails/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> DeanDirectorDetails(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            // ---------------------------------------------------------
            // INSTITUTION
            // ---------------------------------------------------------

            var institution = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(i =>
                    i.CollegeCode == collegeCode);

            if (institution == null)
            {
                return NotFound(
                    $"Institution details not found for college code: {collegeCode}");
            }

            var facultyCode = institution.FacultyCode;


            // ---------------------------------------------------------
            // COLLEGE
            // ---------------------------------------------------------

            var college = await _context.AffiliationCollegeMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.CollegeCode == collegeCode);

            ViewBag.CollegeCode = collegeCode;

            ViewBag.CollegeName =
                college?.CollegeName ?? "Unknown College";

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;

            ViewBag.UserDesignation = GetUserDesignation();

            // ---------------------------------------------------------
            // QUALIFICATIONS
            // ---------------------------------------------------------

            ViewBag.Qualifications = new SelectList(
                await _context.MstCourses
                    .AsNoTracking()
                    .Where(c =>
                        !string.IsNullOrEmpty(c.CourseName) &&
                        c.FacultyCode.ToString() == facultyCode)
                    .OrderBy(c => c.CourseName)
                    .Select(c => new
                    {
                        c.Id,
                        c.CourseName
                    })
                    .ToListAsync(),
                "Id",
                "CourseName"
            );


            // ---------------------------------------------------------
            // EXPERIENCE COLLEGES
            // Includes:
            // 1. AffiliationCollegeMasters
            // 2. AffiliationOthersCollegeMasters
            // ---------------------------------------------------------

            var experienceColleges =
                await GetAllExperienceCollegesAsync(facultyCode);

            ViewBag.ExperienceColleges =
                experienceColleges;

            // Optional - keep this if other parts of the page
            // need a SelectList.
            ViewBag.CollegeList =
                new SelectList(
                    experienceColleges,
                    "Value",
                    "Text"
                );


            // ---------------------------------------------------------
            // DEAN / DIRECTOR
            // ---------------------------------------------------------

            var dean = await _context.AffDeanOrDirectorDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.FacultyCode == facultyCode &&
                    d.CollegeCode == collegeCode);

            if (dean == null)
            {
                return NotFound(
                    "Dean / Director details not found.");
            }


            // ---------------------------------------------------------
            // VIEW MODEL
            // ---------------------------------------------------------

            var vm = new DeanDetailsViewModel
            {
                FacultyCode = facultyCode,
                CollegeCode = collegeCode,

                DeanOrDirectorName =
                    dean.DeanOrDirectorName,

                DeanQualification =
                    dean.DeanQualification,

                DeanQualificationDate =
                    dean.DeanQualificationDate,

                DeanUniversity =
                    dean.DeanUniversity,

                DeanStateCouncilNumber =
                    dean.DeanStateCouncilNumber
            };


            // ---------------------------------------------------------
            // QUALIFICATION NAME
            // ---------------------------------------------------------

            ViewBag.DeanQualificationName = "-";

            if (int.TryParse(
                dean.DeanQualification,
                out int qualificationId))
            {
                ViewBag.DeanQualificationName =
                    await _context.MstCourses
                        .AsNoTracking()
                        .Where(c =>
                            c.Id == qualificationId)
                        .Select(c =>
                            c.CourseName)
                        .FirstOrDefaultAsync()
                    ?? dean.DeanQualification
                    ?? "-";
            }
            else
            {
                ViewBag.DeanQualificationName =
                    dean.DeanQualification ?? "-";
            }


            // ---------------------------------------------------------
            // RECOGNITION
            // ---------------------------------------------------------

            if (facultyCode == "2")
            {
                vm.RecognizedByDCI =
                    dean.RecognizedByDci ?? false;
            }
            else
            {
                vm.RecognizedByMCI =
                    dean.RecognizedByMci ?? false;
            }


            // ---------------------------------------------------------
            // TEACHING EXPERIENCE
            // ---------------------------------------------------------

            vm.TeachingExperiences =
                await _context.AffDeanTeachingExperiences
                    .AsNoTracking()
                    .Where(t =>
                        t.DeanId == dean.Id)
                    .Select(t => new TeachingExperienceRow
                    {
                        Designation =
                            t.Designation,

                        CollegeCode =
                            t.Collegecode,

                        ExpCollegeCode =
                            t.ExpCollegeCode,

                        OtherCollege =
                            t.OtherCollege,

                        FromDate =
                            t.FromDate,

                        ToDate =
                            t.ToDate,

                        TeachingExperienceYears =
                            t.TotalExperienceYears
                    })
                    .ToListAsync();


            // ---------------------------------------------------------
            // ADMINISTRATIVE EXPERIENCE
            // ---------------------------------------------------------

            vm.AdministrativeExperiences =
                await _context.AffDeanAdministrativeExperiences
                    .AsNoTracking()
                    .Where(a =>
                        a.DeanId == dean.Id)
                    .Select(a => new AdministrativeExperienceRow
                    {
                        PostHeld =
                            a.PostHeld,

                        FromDate =
                            a.FromDate,

                        ExpCollegeCode =
                            a.ExpCollegeCode,

                        OtherCollege =
                            a.OtherCollege,

                        ToDate =
                            a.ToDate,

                        TotalExperienceYears =
                            a.TotalExperienceYears
                    })
                    .ToListAsync();


            // ---------------------------------------------------------
            // DEFAULT ROWS
            // ---------------------------------------------------------

            if (!vm.TeachingExperiences.Any())
            {
                vm.TeachingExperiences.Add(
                    new TeachingExperienceRow());
            }

            if (!vm.AdministrativeExperiences.Any())
            {
                vm.AdministrativeExperiences.Add(
                    new AdministrativeExperienceRow());
            }


            // ---------------------------------------------------------
            // VERIFICATION
            // ---------------------------------------------------------

            await SetVerificationViewData<AffDeanOrDirectorDetail>(
                collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 4);

            return View(vm);
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
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<DentalChair>(collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 11);

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

            var designation = GetUserDesignation();

            ViewData["IsAdmin"] = IsAdminUser();

            // =========================================================
            // ADMIN (Admin, Director, Vice Chancellor)
            // =========================================================

            if (IsAdminUser())
            {
                var entity = await _context.Set<T>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        EF.Property<string>(x, property.Name) == collegeCode);

                var history = await _verificationService
                    .GetVerificationHistoryAsync<T>(
                        x => EF.Property<string>(x, property.Name) == collegeCode);

                ViewData["VerificationHistory"] = history;

                if (entity == null)
                {
                    SetPendingVerificationViewData();
                    return;
                }

                // =====================================================
                // GET VERIFICATION PREFIX FROM DESIGNATION
                // =====================================================

                var verificationPrefix = GetVerificationPrefix(designation);

                if (string.IsNullOrWhiteSpace(verificationPrefix))
                {
                    throw new Exception(
                        $"No verification mapping found for designation: {designation}");
                }

                // =====================================================
                // GET DESIGNATION-SPECIFIC PROPERTIES DYNAMICALLY
                // =====================================================

                var properties = typeof(T).GetProperties();

                var statusProperty = properties.FirstOrDefault(p =>
                    p.Name.Equals(
                        $"Is{verificationPrefix}Verified",
                        StringComparison.OrdinalIgnoreCase));

                var remarksProperty = properties.FirstOrDefault(p =>
                    p.Name.Equals(
                        $"{verificationPrefix}Remarks",
                        StringComparison.OrdinalIgnoreCase));

                var dateProperty = properties.FirstOrDefault(p =>
                    p.Name.Equals(
                        $"{verificationPrefix}VerifiedDate",
                        StringComparison.OrdinalIgnoreCase));

                var nameProperty = properties.FirstOrDefault(p =>
                    p.Name.Equals(
                        $"{verificationPrefix}Name",
                        StringComparison.OrdinalIgnoreCase));

                if (statusProperty == null)
                {
                    throw new Exception(
                        $"{typeof(T).Name} does not contain verification properties " +
                        $"for designation: {designation}");
                }

                bool? isVerified =
                    statusProperty.GetValue(entity) as bool?;

                string? remarks =
                    remarksProperty?.GetValue(entity) as string;

                DateTime? verifiedDate =
                    dateProperty?.GetValue(entity) as DateTime?;

                string? verifiedBy =
                    nameProperty?.GetValue(entity) as string;

                // =====================================================
                // NO VERIFICATION YET
                // =====================================================

                if (isVerified == null &&
                    string.IsNullOrWhiteSpace(remarks))
                {
                    SetPendingVerificationViewData();
                    return;
                }

                // =====================================================
                // EXISTING VERIFICATION
                // =====================================================

                ViewData["ExistingRemarks"] = remarks;

                ViewData["ExistingStatus"] =
                    isVerified switch
                    {
                        true => "Approved",
                        false => "Rejected",
                        null => "Pending"
                    };

                ViewData["ExistingStatusClass"] =
                    isVerified switch
                    {
                        true => "bg-success",
                        false => "bg-danger",
                        null => "bg-warning"
                    };

                ViewData["VerifiedBy"] =
                    verifiedBy ?? designation;

                ViewData["VerifiedDate"] =
                    verifiedDate?.ToString("dd-MM-yyyy hh:mm tt");

                ViewData["ShowFeedbackForm"] =
                    isVerified == null;

                return;
            }
            // =========================================================
            // NORMAL USER
            // =========================================================

            var verification = await _verificationService
                .GetVerificationAsync<T>(
                    x => EF.Property<string>(x, property.Name) == collegeCode,
                    designation);

            // Also fetch full verification history for display
            var verificationHistory = await _verificationService
                .GetVerificationHistoryAsync<T>(
                    x => EF.Property<string>(x, property.Name) == collegeCode);

            if (verification == null)
            {
                ViewData["ExistingRemarks"] = null;
                ViewData["ExistingStatus"] = "Pending";
                ViewData["ExistingStatusClass"] = "bg-warning";
                ViewData["VerifiedBy"] = null;
                ViewData["VerifiedDate"] = null;
                ViewData["ShowFeedbackForm"] = true;

                ViewData["VerificationHistory"] = verificationHistory;

                return;
            }

            ViewData["ExistingRemarks"] = verification.Remarks;

            ViewData["ExistingStatus"] =
                verification.IsVerified switch
                {
                    true => "Approved",
                    false => "Rejected",
                    null => "Pending"
                };

            ViewData["ExistingStatusClass"] =
                verification.IsVerified switch
                {
                    true => "bg-success",
                    false => "bg-danger",
                    null => "bg-warning"
                };

            ViewData["VerifiedBy"] = verification.VerifiedBy;

            ViewData["VerifiedDate"] = verification.VerifiedDate?.ToString("dd-MM-yyyy hh:mm tt");

            ViewData["ShowFeedbackForm"] = verification.IsVerified == null;

            ViewData["VerificationHistory"] = verificationHistory;
        }



        private string? GetVerificationPrefix(string? designation)
        {
            if (string.IsNullOrWhiteSpace(designation))
                return null;

            return designation.Trim().ToLowerInvariant() switch
            {
                "deo" => "Deo",
                "data entry operator" => "Deo",

                "jr" => "Jr",
                "junior registrar" => "Jr",

                "so" => "So",
                "section officer" => "So",

                "ar" => "Ar",
                "assistant registrar" => "Ar",

                "rg" => "Rg",
                "registrar" => "Rg",

                "re" => "Re",

                "dr" => "Dr",
                "director" => "Dr",

                "vc" => "Vc",
                "vice chancellor" => "Vc",

                _ => null
            };
        }

        private void SetPendingVerificationViewData()
        {
            ViewData["ExistingRemarks"] = null;

            ViewData["ExistingStatus"] = "Pending";

            ViewData["ExistingStatusClass"] =
                "bg-warning";

            ViewData["VerifiedBy"] = null;

            ViewData["VerifiedDate"] = null;

            ViewData["ShowFeedbackForm"] = true;
        }

        // GET: /VerificationDashboard/PrincipalDetails/{collegeCode}
        [HttpGet]
        public async Task<IActionResult> PrincipalDetails(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var context = await GetPageContextAsync(collegeCode);

            PopulateCommonViewBags(context);

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();


            // ---------------------------------------------------------
            // INSTITUTION
            // ---------------------------------------------------------

            var institution = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode);

            if (institution == null)
            {
                return NotFound(
                    $"Institution details not found for college code: {collegeCode}");
            }

            var facultyCode = institution.FacultyCode;


            // ---------------------------------------------------------
            // COLLEGE NAME
            // ---------------------------------------------------------

            var college = await _context.AffiliationCollegeMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode);

            ViewBag.CollegeName =
                college?.CollegeName ?? "Unknown College";


            // ---------------------------------------------------------
            // EXPERIENCE COLLEGE LIST
            // ---------------------------------------------------------

            var experienceColleges =
                await GetAllExperienceCollegesAsync(facultyCode);

            ViewBag.ExperienceColleges =
                experienceColleges;


            // ---------------------------------------------------------
            // PRINCIPAL
            // ---------------------------------------------------------

            var principal = await _context.AffPrincipalDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode);

            if (principal == null)
            {
                return NotFound(
                    $"Principal details not found for college code: {collegeCode}");
            }


            // ---------------------------------------------------------
            // QUALIFICATION NAME
            // ---------------------------------------------------------

            string principalQualification = "-";

            if (int.TryParse(
                principal.DeanQualification,
                out int qualificationId))
            {
                principalQualification =
                    await _context.MstCourses
                        .AsNoTracking()
                        .Where(c => c.Id == qualificationId)
                        .Select(c => c.CourseName)
                        .FirstOrDefaultAsync()
                    ?? principal.DeanQualification
                    ?? "-";
            }
            else
            {
                principalQualification =
                    principal.DeanQualification ?? "-";
            }


            // ---------------------------------------------------------
            // PRINCIPAL BASIC INFORMATION
            // ---------------------------------------------------------

            ViewBag.PrincipalName =
                principal.DeanOrDirectorName ?? "-";

            ViewBag.PrincipalQualification =
                principalQualification;

            ViewBag.PrincipalQualificationDate =
                principal.DeanQualificationDate?
                    .ToString("dd-MM-yyyy") ?? "-";

            ViewBag.PrincipalUniversity =
                principal.DeanUniversity ?? "-";

            ViewBag.PrincipalStateCouncilNumber =
                principal.DeanStateCouncilNumber ?? "-";


            // ---------------------------------------------------------
            // RECOGNITION
            // ---------------------------------------------------------

            if (facultyCode == "2")
            {
                ViewBag.RecognitionLabel =
                    "Recognized by DCI";

                ViewBag.RecognitionStatus =
                    principal.RecognizedByDci == true
                        ? "Yes"
                        : principal.RecognizedByDci == false
                            ? "No"
                            : "-";
            }
            else
            {
                ViewBag.RecognitionLabel =
                    "Recognized by MCI";

                ViewBag.RecognitionStatus =
                    principal.RecognizedByMci == true
                        ? "Yes"
                        : principal.RecognizedByMci == false
                            ? "No"
                            : "-";
            }


            // ---------------------------------------------------------
            // TEACHING EXPERIENCE
            // ---------------------------------------------------------

            var teachingExperiences =
                await _context.AffPrincipalTeachingExperiences
                    .AsNoTracking()
                    .Where(t =>
                        t.DeanId == principal.Id)
                    .OrderBy(t => t.Id)
                    .ToListAsync();


            ViewBag.TeachingExperiences =
                teachingExperiences;


            // ---------------------------------------------------------
            // ADMINISTRATIVE EXPERIENCE
            // ---------------------------------------------------------

            var administrativeExperiences =
                await _context.AffPrincipalAdministrativeExperiences
                    .AsNoTracking()
                    .Where(a =>
                        a.DeanId == principal.Id)
                    .OrderBy(a => a.Id)
                    .ToListAsync();


            ViewBag.AdministrativeExperiences =
                administrativeExperiences;


            // ---------------------------------------------------------
            // VERIFICATION
            // ---------------------------------------------------------

            await SetVerificationViewData<AffPrincipalDetail>(
                collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 5);

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
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var context = await GetPageContextAsync(collegeCode);

            PopulateCommonViewBags(context);

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();


            // ---------------------------------------------------------
            // PG / SUPER SPECIALTY COURSES
            // ---------------------------------------------------------

            var courses =
                await _context.AffiliationPgSsCourseDetails
                    .AsNoTracking()
                    .Where(c => c.CollegeCode == collegeCode)
                    .OrderBy(c => c.CourseName)
                    .ToListAsync();


            // ---------------------------------------------------------
            // COLLEGE
            // ---------------------------------------------------------

            var college =
                await _context.AffiliationCollegeMasters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c =>
                        c.CollegeCode == collegeCode);

            ViewBag.CollegeName =
                college?.CollegeName
                ?? "Unknown College";


            // ---------------------------------------------------------
            // INSTITUTION
            // ---------------------------------------------------------

            var institution =
                await _context.AffInstitutionsDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i =>
                        i.CollegeCode == collegeCode);

            ViewBag.InstitutionName =
                institution?.NameOfInstitution
                ?? "Unknown Institution";


            // ---------------------------------------------------------
            // PAGE INFORMATION
            // ---------------------------------------------------------

            ViewBag.TabTitle =
                "PG / Super Specialty Course Details";

            ViewBag.TabIcon =
                "bi-mortarboard";


            // ---------------------------------------------------------
            // NAVIGATION
            // ---------------------------------------------------------

            ViewBag.NextTabAction =
                Url.Action(
                    "Infrastructure",
                    "VerificationDashboard",
                    new { collegeCode });

            ViewBag.NextTabLabel =
                "Next: Infrastructure";

            ViewBag.PrevTabAction =
                Url.Action(
                    "UgCourseDetails",
                    "VerificationDashboard",
                    new { collegeCode });

            ViewBag.PrevTabLabel =
                "Previous: UG Course Details";


            // ---------------------------------------------------------
            // VERIFICATION
            // ---------------------------------------------------------

            await SetVerificationViewData<AffiliationPgSsCourseDetail>(
                collegeCode);
            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 7);

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
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
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

                { "CA_VehicleDetails", typeof(CaVehicleDetail) },

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
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.NextTabAction = Url.Action("PgCourseDetails", new { collegeCode });
            ViewBag.NextTabLabel = "Next: PG Course Details";
            ViewBag.PrevTabAction = Url.Action("PrincipalDetails", new { collegeCode });
            ViewBag.PrevTabLabel = "Previous: Principal Details";
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<AffiliationCourseDetail>(collegeCode);
            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 6);
            return View(ugCourses);
        }

        [HttpGet]
        public async Task<IActionResult> PreviousNotification(string collegeCode, string courseId)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            if (string.IsNullOrWhiteSpace(courseId))
                return NotFound("Course ID is required.");

            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var context = await GetPageContextAsync(collegeCode);

            var facultyCode = context.Institution.FacultyCode;

            if (string.IsNullOrWhiteSpace(facultyCode))
                return NotFound("Faculty code not found.");

            // ---------------------------------------------------------
            // COURSE
            // ---------------------------------------------------------

            var course = await _context.AffiliationCourseDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Facultycode == facultyCode &&
                    x.Collegecode == collegeCode &&
                    x.CourseId == courseId);

            if (course == null)
                return NotFound("Course details not found.");

            // ---------------------------------------------------------
            // DOCUMENT
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(course.PreviousNotificationFilesPath))
                return NotFound("Previous notification file not found.");

            if (!System.IO.File.Exists(course.PreviousNotificationFilesPath))
                return NotFound("Previous notification file not found.");

            // ---------------------------------------------------------
            // INLINE DOCUMENT VIEWER
            // ---------------------------------------------------------

            Response.Headers["Content-Disposition"] = "inline";

            return PhysicalFile(
                course.PreviousNotificationFilesPath,
                "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ViewGOKOrder(string collegeCode, string courseId)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            if (string.IsNullOrWhiteSpace(courseId))
                return NotFound("Course ID is required.");

            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var context = await GetPageContextAsync(collegeCode);

            var facultyCode = context.Institution.FacultyCode;

            if (string.IsNullOrWhiteSpace(facultyCode))
                return NotFound("Faculty code not found.");

            // ---------------------------------------------------------
            // COURSE
            // ---------------------------------------------------------

            var entity = await _context.AffiliationCourseDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Facultycode == facultyCode &&
                    x.Collegecode == collegeCode &&
                    x.CourseId == courseId);

            if (entity == null)
                return NotFound("Course details not found.");

            // ---------------------------------------------------------
            // DOCUMENT
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(entity.GokorderPath) ||
                !System.IO.File.Exists(entity.GokorderPath))
            {
                return NotFound("GOK Order file not found.");
            }

            // ---------------------------------------------------------
            // INLINE DOCUMENT VIEWER
            // ---------------------------------------------------------

            Response.Headers["Content-Disposition"] = "inline";

            return PhysicalFile(
                entity.GokorderPath,
                "application/pdf");
        }


        [HttpGet]
        public async Task<IActionResult> ViewLastAffiliation( string collegeCode,  string courseId)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            if (string.IsNullOrWhiteSpace(courseId))
                return NotFound("Course ID is required.");

            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var context = await GetPageContextAsync(collegeCode);

            var facultyCode = context.Institution.FacultyCode;

            if (string.IsNullOrWhiteSpace(facultyCode))
                return NotFound("Faculty code not found.");

            // ---------------------------------------------------------
            // COURSE
            // ---------------------------------------------------------

            var entity = await _context.AffiliationCourseDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Facultycode == facultyCode &&
                    x.Collegecode == collegeCode &&
                    x.CourseId == courseId);

            if (entity == null)
                return NotFound("Course details not found.");

            // ---------------------------------------------------------
            // DOCUMENT
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(entity.LastAffiliationRguhsfilePath) ||
                !System.IO.File.Exists(entity.LastAffiliationRguhsfilePath))
            {
                return NotFound("RGUHS notification file not found.");
            }

            // ---------------------------------------------------------
            // INLINE DOCUMENT VIEWER
            // ---------------------------------------------------------

            Response.Headers["Content-Disposition"] = "inline";

            return PhysicalFile(
                entity.LastAffiliationRguhsfilePath,
                "application/pdf");
        }


        [HttpGet]
        public async Task<IActionResult> LandAndBuildingDetails(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            // ---------------------------------------------------------
            // PAGE CONTEXT
            // ---------------------------------------------------------

            var context = await GetPageContextAsync(collegeCode);

            PopulateCommonViewBags(context);

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();


            // ---------------------------------------------------------
            // COLLEGE
            // ---------------------------------------------------------

            var college = await _context.AffiliationCollegeMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.CollegeCode == collegeCode);

            ViewBag.CollegeName =
                college?.CollegeName ?? "Unknown College";


            // ---------------------------------------------------------
            // INSTITUTION
            // ---------------------------------------------------------

            var institution = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(i =>
                    i.CollegeCode == collegeCode);

            ViewBag.InstitutionName =
                institution?.NameOfInstitution
                ?? "Unknown Institution";


            // ---------------------------------------------------------
            // LAND & BUILDING
            // ---------------------------------------------------------

            var landBuilding =
                await _context.DentalCollegeLandBuildingDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.CollegeCode == collegeCode);

            if (landBuilding == null)
            {
                return NotFound(
                    $"Land & Building details not found for college code: {collegeCode}");
            }


            // ---------------------------------------------------------
            // VERIFICATION
            // ---------------------------------------------------------

            await SetVerificationViewData<DentalCollegeLandBuildingDetail>(
                collegeCode);


            // ---------------------------------------------------------
            // NAVIGATION
            // ---------------------------------------------------------

            ViewBag.PrevTabAction = Url.Action(
                "PgCourseDetails",
                "VerificationDashboard",
                new { collegeCode });

            ViewBag.PrevTabLabel =
                "Previous: PG Course Details";

            ViewBag.NextTabAction = Url.Action(
                "ClassroomAndLaboratory",
                "VerificationDashboard",
                new { collegeCode });

            ViewBag.NextTabLabel =
                "Next: Classroom & Laboratory";

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 9);

            return View(landBuilding);
        }


        [HttpGet]
        public async Task<IActionResult> ViewLandBuildingDocument( int id, string documentType)
        {
            var record = await _context.DentalCollegeLandBuildingDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (record == null)
                return NotFound();

            string? filePath = documentType switch
            {
                "SaleDeed" =>
                    record.SaleDeedDocumentPath,

                "EncumbranceCertificate" =>
                    record.EncumbranceCertificateDocumentPath,

                "LandUseCertificate" =>
                    record.LandUseCertificateDocumentPath,

                "ApprovedLayoutPlan" =>
                    record.ApprovedLayoutPlanDocumentPath,

                "LandSketch" =>
                    record.LandSketchDocumentPath,

                "DistanceCertificate" =>
                    record.DistanceCertificateDocumentPath,

                "ApprovedBuildingPlan" =>
                    record.ApprovedBuildingPlanDocumentPath,

                "CompletionCertificate" =>
                    record.CompletionCertificateDocumentPath,

                "StructuralStabilityCertificate" =>
                    record.StructuralStabilityCertificateDocumentPath,

                "FireSafetyNoc" =>
                    record.FireSafetyNocDocumentPath,

                "LiftLicense" =>
                    record.LiftLicenseDocumentPath,

                "ElectricalSafetyCertificate" =>
                    record.ElectricalSafetyCertificateDocumentPath,

                "WaterSupplyCertificate" =>
                    record.WaterSupplyCertificateDocumentPath,

                "SewageSanitationApproval" =>
                    record.SewageSanitationApprovalDocumentPath,

                _ => null
            };

            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound("Document not available.");

            if (!System.IO.File.Exists(filePath))
                return NotFound("Document file not found.");

            return PhysicalFile(
                filePath,
                GetContentType(filePath),
                enableRangeProcessing: true);
        }


        [HttpGet]
        public async Task<IActionResult> ViewApprovedBuildingPlanDocument(int id)
        {
            var record = await _context.DentalCollegeLandBuildingDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (record == null)
                return NotFound();

            var filePath = record.ApprovedBuildingPlanDocumentPath;

            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound("Approved Building Plan document not available.");

            // If the database stores the complete physical path,
            // this check is enough.
            if (!System.IO.File.Exists(filePath))
                return NotFound("Approved Building Plan file not found.");

            return PhysicalFile(
                filePath,
                GetContentType(filePath),
                enableRangeProcessing: true);
        }

        private static string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => "application/pdf",

                ".jpg" or ".jpeg" => "image/jpeg",

                ".png" => "image/png",

                ".gif" => "image/gif",

                ".webp" => "image/webp",

                ".txt" => "text/plain",

                _ => "application/octet-stream"
            };
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
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();

            VerificationDisplayModel? verification = null;

            bool hasSectionData = savedInfrastructure.Any();

            try
            {
                verification = await _verificationService
                        .GetVerificationAsync<DentalInfrastructure>(
                            x => x.CollegeCode == collegeCode,
                            GetUserDesignation());
            }
            catch (Exception ex) when (
                ex.Message.Contains("record not found"))
            {
                verification = null;
            }

            SetVerificationViewData( verification, hasSectionData, "Classroom and Laboratory details have not been submitted for this college.");

            ViewBag.ActiveTab = "ClassroomAndLaboratory";
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

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 10);

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

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<DentalCollegeEquipmentDetail>(
                collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 15);

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
            ViewBag.IsAdmin = IsAdminUser();
            ViewBag.IsSection = IsSectionUser();
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
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<MedicalUgbedDistribution>(collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 12);

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> HostelDetails(string collegeCode)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code is required.");

            var context = await GetPageContextAsync(collegeCode);

            PopulateCommonViewBags(context);

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
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

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 13);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ViewHostelDocument( string collegeCode, string documentType)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound();

            if (documentType != "PossessionProof")
                return NotFound();

            var hostel = await _context.AffHostelDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode);

            if (hostel == null)
                return NotFound();

            var filePath = hostel.PossessionProofPath;

            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound();

            var resolvedPath = ResolveDocumentPath(filePath);

            if (!System.IO.File.Exists(resolvedPath))
                return NotFound();

            return PhysicalFile(
                resolvedPath,
                GetDocumentContentType(resolvedPath));
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

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
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


            await SetVerificationViewData<AffInstitutionsDetail>( collegeCode);
            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 14);
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
            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 16);

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

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();

            // Existing verification information
            await SetVerificationViewData<CaAcademicPerformance>( collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 17);


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
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.UserDesignation = GetUserDesignation();

            await SetVerificationViewData<CaAcademicPerformance>(
                collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 18);

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

            ViewBag.InstitutionName = pageContext.Institution.NameOfInstitution;
            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = facultyCode;
            ViewBag.AffiliationType = affiliationType;
            ViewBag.CourseLevel = courseLevel;
            ViewBag.CourseLevels = levels;
            ViewBag.IsDentalFaculty = facultyCode == 2;
            ViewBag.DepartmentMasters = departmentMasters;
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;


            // =========================================================
            // VERIFICATION DATA
            // =========================================================

            await SetVerificationViewData<CaMedicalDepartmentLibrary>(
                collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 22);

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
                PublicationsNo = researchRecord?.PublicationsNo,
                PublicationsPdfName = researchRecord?.PublicationsPdfName,
                ClinicalTrialsPdfName = researchRecord?.ClinicalTrialsPdfName,
                StudentsRGUHSFunded = researchRecord?.StudentsRguhsfunded,
                StudentsExternalBodyFunding = researchRecord?.StudentsExternalBodyFunding,
                StudentsProjectsPdfName = researchRecord?.StudentsProjectsPdfName,
                FacultyRGUHSFunded = researchRecord?.FacultyRguhsfunded,
                FacultyExternalBodyFunding = researchRecord?.FacultyExternalBodyFunding,
                FacultyProjectsPdfName = researchRecord?.FacultyProjectsPdfName,
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

            ViewBag.InstitutionName = pageContext.Institution.NameOfInstitution;
            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = facultyCode;
            ViewBag.AffiliationType = affiliationType;
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;


            // =========================================================
            // 5. VERIFICATION DATA
            // =========================================================

            await SetVerificationViewData<CaMedResearchPublicationsDetail>(
                collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 23);
            return View("ResearchPublications", model);

        }


        [HttpGet] public async Task<IActionResult> ViewResearchPublicationPdf(string collegeCode) => await GetResearchPdf(collegeCode, "Publications");
        [HttpGet] public async Task<IActionResult> ViewStudentsProjectsPdf(string collegeCode) => await GetResearchPdf(collegeCode, "StudentsProjects");
        [HttpGet] public async Task<IActionResult> ViewFacultyProjectsPdf(string collegeCode) => await GetResearchPdf(collegeCode, "FacultyProjects");
        [HttpGet] public async Task<IActionResult> ViewClinicalTrialsPdf(string collegeCode) => await GetResearchPdf(collegeCode, "ClinicalTrials");

        [HttpGet]
        public IActionResult ViewFacultyDocument( int id, string type, string mode = "view")
        {
            var faculty = _context.FacultyDetails
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == id);

            if (faculty == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(type))
                return BadRequest("Document type is required.");

            string? filePath = type.ToLowerInvariant() switch
            {
                "pg" => faculty.GuideRecognitionDocPath,
                "phd" => faculty.PhDrecognitionDocPath,
                "litig" => faculty.LitigationDocPath,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound("Document not uploaded.");

            // Resolve old/new stored paths against the configured document root
            var resolvedPath = ResolveDocumentPath(filePath);

            if (!System.IO.File.Exists(resolvedPath))
                return NotFound("Document not found on server.");

            var fileName = Path.GetFileName(resolvedPath);

            var contentType = GetDocumentContentType(resolvedPath);

            // Explicit download request
            if (string.Equals(
                mode,
                "download",
                StringComparison.OrdinalIgnoreCase))
            {
                return PhysicalFile(
                    resolvedPath,
                    contentType,
                    fileName);
            }

            // Normal request:
            // Global document viewer fetches this URL and displays it in the popup.
            return PhysicalFile(
                resolvedPath,
                contentType);
        }

        private async Task<IActionResult> GetResearchPdf(  string collegeCode, string fileType)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound();

            var record =
                await _context.CaMedResearchPublicationsDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.CollegeCode == collegeCode);

            if (record == null)
                return NotFound("Record not found.");

            string? filePath = fileType switch
            {
                "Publications" =>
                    record.PublicationsPdfPath,

                "StudentsProjects" =>
                    record.StudentsProjectsPdfPath,

                "FacultyProjects" =>
                    record.FacultyProjectsPdfPath,

                "ClinicalTrials" =>
                    record.ClinicalTrialsPdfPath,

                _ => null
            };

            string? fileName = fileType switch
            {
                "Publications" =>
                    record.PublicationsPdfName,

                "StudentsProjects" =>
                    record.StudentsProjectsPdfName,

                "FacultyProjects" =>
                    record.FacultyProjectsPdfName,

                "ClinicalTrials" =>
                    record.ClinicalTrialsPdfName,

                _ => null
            };

            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound("Document path not found.");

            var resolvedPath =
                ResolveDocumentPath(filePath);

            if (!System.IO.File.Exists(resolvedPath))
                return NotFound(
                    "File not found on server.");

            var finalName =
                string.IsNullOrWhiteSpace(fileName)
                    ? Path.GetFileName(resolvedPath)
                    : fileName;

            Response.Headers["Content-Disposition"] =
                $"inline; filename=\"{finalName}\"";

            return PhysicalFile(
                resolvedPath,
                GetDocumentContentType(resolvedPath));
        }

        [HttpGet]
        public async Task<IActionResult> ViewDepartmentPublicationPdf( string collegeCode, int id)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound();

            var publication = await _context.DeptWisePublications
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CollegeCode == collegeCode);

            if (publication == null)
                return NotFound("Publication not found.");

            if (string.IsNullOrWhiteSpace(publication.PublicationPath))
                return NotFound("PDF not found.");

            var resolvedPath = ResolveDocumentPath(
                publication.PublicationPath);

            if (!System.IO.File.Exists(resolvedPath))
                return NotFound("File does not exist on server.");

            return PhysicalFile(
                resolvedPath,
                GetDocumentContentType(resolvedPath));
        }

        [HttpGet]
        public async Task<IActionResult> ViewDentalLibraryRecord( string collegeCode, int recordId)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound();

            var affiliationType =
                HttpContext.Session.GetInt32("AffiliationType") ?? 2;

            var record = await _context.CaDentalLibraryRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.AffiliationType == affiliationType &&
                    x.RecordId == recordId);

            if (record == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(record.FilePath))
                return NotFound();

            var resolvedPath = ResolveDocumentPath(record.FilePath);

            if (!System.IO.File.Exists(resolvedPath))
                return NotFound();

            return PhysicalFile(
                resolvedPath,
                GetDocumentContentType(resolvedPath));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSectionFeedback( SectionFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var referer = Request.Headers.Referer.ToString();

                if (!string.IsNullOrWhiteSpace(referer))
                {
                    var uri = new Uri(referer);
                    var localUrl = uri.PathAndQuery;

                    if (Url.IsLocalUrl(localUrl))
                    {
                        return Redirect(localUrl);
                    }
                }

                return RedirectToAction(
                    "InstitutionDetails",
                    new { collegeCode = model.CollegeCode }
                );
            }

            var feedback = await _context.SectionWiseFeedbacks
                .FirstOrDefaultAsync(x =>
                    x.FacultyId == model.FacultyId &&
                    x.CollegeCode == model.CollegeCode &&
                    x.TabId == model.TabId &&
                    x.SectionId == model.SectionId);

            bool isNew = feedback == null;

            if (feedback == null)
            {
                feedback = new SectionWiseFeedback
                {
                    FacultyId = model.FacultyId,
                    CollegeCode = model.CollegeCode,
                    TabId = model.TabId,
                    SectionId = model.SectionId
                };

                _context.SectionWiseFeedbacks.Add(feedback);
            }

            feedback.VerificationStatus = model.VerificationStatus;
            feedback.Remarks = model.Remarks;
            feedback.VerifiedBy = User.Identity?.Name;
            feedback.VerifiedOn = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = isNew ? "Section verification saved successfully." : "Section verification updated successfully.";

            // Redirect back to the page from which the form was submitted
            var refererUrl = Request.Headers.Referer.ToString();

            if (!string.IsNullOrWhiteSpace(refererUrl))
            {
                var uri = new Uri(refererUrl);
                var localUrl = uri.PathAndQuery;

                if (Url.IsLocalUrl(localUrl))
                {
                    return Redirect(localUrl);
                }
            }

            // Fallback
            return RedirectToAction(
                "InstitutionDetails",
                new { collegeCode = model.CollegeCode }
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

                ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 21);

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
        public async Task<IActionResult> ViewGoverningCouncilPdf( string collegeCode, string courseLevel, string facultyCode)
        {
            return await GetPdf(
                "GoverningCouncil",
                collegeCode,
                facultyCode,
                courseLevel);
        }

        [HttpGet]
        public async Task<IActionResult> ViewAccountSummaryPdf( string collegeCode, string courseLevel, string facultyCode)
        {
            return await GetPdf( "AccountSummary", collegeCode, facultyCode, courseLevel);
        }

        [HttpGet]
        public async Task<IActionResult> ViewAuditedStatementPdf( string collegeCode, string courseLevel, string facultyCode)
        {
            return await GetPdf( "AuditedStatement", collegeCode, facultyCode, courseLevel);
        }

        [HttpGet]
        public async Task<IActionResult> ViewDonationPdf( string collegeCode, string courseLevel, string facultyCode)
        {
            return await GetPdf("Donation", collegeCode, facultyCode, courseLevel);
        }

        private async Task<IActionResult> GetPdf( string type, string collegeCode, string facultyCode, string courseLevel)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound("College code not specified.");

            if (string.IsNullOrWhiteSpace(facultyCode))
                return NotFound("Faculty code not specified.");

            if (string.IsNullOrWhiteSpace(courseLevel))
                return NotFound("Course level not specified.");

            var record = await _context.MedCaAccountAndFeeDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode &&
                    x.CourseLevel == courseLevel);

            if (record == null)
                return NotFound("Record not found.");

            string? filePath = type switch
            {
                "GoverningCouncil" => record.GoverningCouncilPdfPath,
                "AccountSummary" => record.AccountSummaryPdfPath,
                "AuditedStatement" => record.AuditedStatementPdfPath,
                "Donation" => record.DonationPdfPath,
                _ => null
            };

            string? fileName = type switch
            {
                "GoverningCouncil" => record.GoverningCouncilPdfName,
                "AccountSummary" => record.AccountSummaryPdfName,
                "AuditedStatement" => record.AuditedStatementPdfName,
                "Donation" => record.DonationPdfName,
                _ => null
            };

            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound("File path not found.");

            var resolvedPath = ResolveDocumentPath(filePath);

            if (!System.IO.File.Exists(resolvedPath))
                return NotFound("File not found on server.");

            var finalName = string.IsNullOrWhiteSpace(fileName)
                ? Path.GetFileName(resolvedPath)
                : fileName;

            Response.Headers["Content-Disposition"] =
                $"inline; filename=\"{finalName}\"";

            return PhysicalFile(
                resolvedPath,
                GetDocumentContentType(resolvedPath));
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
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 19);
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

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 20);

            return View("StaffOtherDetails", vm);
        }



        public IActionResult DataNotAvailable( string entityName,  string collegeCode, string pageName)
        {
            var model = new VerificationDataNotFoundViewModel
            {
                EntityName = entityName,
                CollegeCode = collegeCode,
                PageName = pageName,

                Message =
                    $"The required data for {pageName} has not been entered for this college yet."
            };

            return View(model);
        }

        private void SetVerificationViewData( VerificationDisplayModel? verification, bool hasSectionData, string noDataMessage)
        {
            // ==========================================
            // NO DATA AVAILABLE
            // ==========================================

            if (!hasSectionData)
            {
                ViewData["NoDataAvailable"] = true;
                ViewData["NoDataMessage"] = noDataMessage;

                ViewData["ExistingRemarks"] = null;
                ViewData["ExistingStatus"] = "Not Submitted";
                ViewData["ExistingStatusClass"] = "bg-secondary";
                ViewData["VerifiedBy"] = null;
                ViewData["VerifiedDate"] = null;
                ViewData["ShowFeedbackForm"] = false;

                return;
            }

            // ==========================================
            // DATA AVAILABLE
            // ==========================================

            ViewData["NoDataAvailable"] = false;

            ViewData["ExistingRemarks"] = verification?.Remarks;

            ViewData["ExistingStatus"] = verification?.IsVerified switch
            {
                true => "Approved",
                false => "Rejected",
                null => "Pending"
            };

            ViewData["ExistingStatusClass"] = verification?.IsVerified switch
            {
                true => "bg-success",
                false => "bg-danger",
                null => "bg-warning"
            };

            ViewData["VerifiedBy"] = verification?.VerifiedBy;

            ViewData["VerifiedDate"] =
                verification?.VerifiedDate?.ToString("dd-MM-yyyy hh:mm tt");

            ViewData["ShowFeedbackForm"] =
                verification?.IsVerified == null;
        }

        [HttpGet]
        public async Task<IActionResult> ViewStaffOtherPdf( string collegeCode, string fileType)
        {
            if (string.IsNullOrWhiteSpace(collegeCode))
                return NotFound();

            var courseLevel = "UG";

            var record = await _context.CaMedStaffParticularsOthers
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.CourseLevel == courseLevel);

            if (record == null)
                return NotFound();

            string? filePath = fileType switch
            {
                "Examiner" =>
                    record.ExaminerDetailsPdfPath,

                "Examiner2" =>
                    record.ExaminerDetailsPdfPath2,

                "Examiner3" =>
                    record.ExaminerDetailsPdfPath3,

                "Examiner4" =>
                    record.ExaminerDetailsPdfPath4,

                "Examiner5" =>
                    record.ExaminerDetailsPdfPath5,

                "AEBAS3Months" =>
                    record.AebaslastThreeMonthsPdfPath,

                "AEBASInspection" =>
                    record.AebasinspectionDayPdfPath,

                "PF" =>
                    record.ProvidentFundPdfPath,

                "ESI" =>
                    record.EsipdfPath,

                _ => null
            };

            if (string.IsNullOrWhiteSpace(filePath))
                return NotFound();

            var resolvedPath = ResolveDocumentPath(filePath);

            if (!System.IO.File.Exists(resolvedPath))
                return NotFound("File not found");

            return PhysicalFile(
                resolvedPath,
                GetDocumentContentType(resolvedPath));
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

            ViewBag.InstitutionName = pageContext.Institution.NameOfInstitution;
            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = facultyCode;
            ViewBag.AffiliationType = affiliationType;
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;


            // ---------------------------------------------------------
            // VERIFICATION DATA
            // ---------------------------------------------------------

            await SetVerificationViewData<CaMedLibraryGeneral>(
                collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 24);

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

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 25);

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

            ViewBag.InstitutionName = pageContext.Institution.NameOfInstitution;
            ViewBag.CollegeCode = collegeCode;
            ViewBag.FacultyCode = facultyCode;
            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
            ViewBag.AffiliationType = affiliationType;


            // ---------------------------------------------------------
            // VERIFICATION DATA
            // ---------------------------------------------------------

            await SetVerificationViewData<TeachingStaffDepartmentWiseDetail>(
                collegeCode);

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 26);

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

            ViewBag.ActiveTab = ControllerContext.ActionDescriptor.ActionName;
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

            ViewBag.SectionFeedback = await GetTabSectionFeedbackAsync(collegeCode, 27);

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

        // Helper method to check if current user is admin
        private bool IsAdminUser()
        {
            var isAdminClaim = User.FindFirst("IsAdmin");
            return isAdminClaim != null && bool.TryParse(isAdminClaim.Value, out bool isAdmin) && isAdmin;
        }

        // Helper method to check if current user is section officer
        private bool IsSectionUser()
        {
            var isSectionClaim = User.FindFirst("IsSection");
            return isSectionClaim != null && bool.TryParse(isSectionClaim.Value, out bool isSection) && isSection;
        }
    }
}