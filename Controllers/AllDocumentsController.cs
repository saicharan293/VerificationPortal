using Microsoft.AspNetCore.Mvc;
using VerificationPortal.DATA;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Controllers
{
    public class AllDocumentsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IVerificationService _verificationService;
        private readonly IClinicalFacilitiesCompositeService _clinicalFacilitiesCompositeService;
        private static readonly int?[] _yearIds = { 1, 2, 3, 4 };

        public AllDocumentsController(ApplicationDbContext context, IVerificationService verificationService, IClinicalFacilitiesCompositeService clinicalFacilitiesCompositeService)
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

        private string ResolveDocumentPath(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return string.Empty;

            filePath = filePath.Replace('/', '\\');

            // Existing absolute path
            if (Path.IsPathRooted(filePath) && System.IO.File.Exists(filePath))
            {
                return filePath;
            }

            // Remove the stored BaseDentalPath if it is already included
            var basePath = BaseDentalPath.TrimEnd('\\');

            if (filePath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                filePath = filePath.Substring(basePath.Length).TrimStart('\\');
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

        [HttpGet]
        public IActionResult ViewGoverningBodyCouncilDocument(int id)
        {
            var institution = _context.AffInstitutionsDetails
                .FirstOrDefault(x => x.InstitutionId == id);

            if (institution == null)
                return NotFound();

            var storedPath = institution.MembersOfGoverningBodyOrCouncilFilePath;

            if (string.IsNullOrWhiteSpace(storedPath))
                return NotFound();

            var filePath = ResolveDocumentPath(storedPath);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(filePath, GetDocumentContentType(filePath));
        }

        [HttpGet]
        public IActionResult ViewDeanAppointmentOrderDocument(int id)
        {
            var institution = _context.AffInstitutionsDetails
                .FirstOrDefault(x => x.InstitutionId == id);

            if (institution == null)
                return NotFound();

            var storedPath = institution.DocumentDataPath;

            if (string.IsNullOrWhiteSpace(storedPath))
                return NotFound();

            var filePath = ResolveDocumentPath(storedPath);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(filePath, GetDocumentContentType(filePath));
        }

        [HttpGet]
        public IActionResult ViewGovAutonomousCertificate(int id)
        {
            var institution = _context.AffInstitutionsDetails.FirstOrDefault(x => x.InstitutionId == id);

            if (institution == null)
                return NotFound();

            var storedPath = institution.GovAutonomousCertPath;

            if (string.IsNullOrWhiteSpace(storedPath))
                return NotFound();

            var filePath = ResolveDocumentPath(storedPath);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(filePath, GetDocumentContentType(filePath));
        }

    }
}
