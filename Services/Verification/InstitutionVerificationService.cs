using Azure.Core;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification
{
    public class InstitutionVerificationService
        : IInstitutionVerificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IVerificationPageService _verificationPageService;
        private readonly IVerificationService _verificationService;

        public InstitutionVerificationService(ApplicationDbContext context, IVerificationPageService verificationPageService, IVerificationService verificationService)
        {
            _context = context;
            _verificationPageService = verificationPageService;
            _verificationService = verificationService;
        }

        private class InstitutionLookupData
        {
            public string TypeOfInstitution { get; set; } = string.Empty;

            public string StatusOfCollege { get; set; } = string.Empty;

            public string Taluk { get; set; } = string.Empty;

            public string District { get; set; } = string.Empty;
        }

        private class InstitutionDocumentIds
        {
            public int? MemberOfGovBodyDocumentId { get; set; }

            public int? AppointmentOrderDocumentId { get; set; }

            public int? GovAutonomousDocumentId { get; set; }
        }

        public async Task<InstitutionDetailsVerificationVm?> GetInstitutionDetailsAsync( string collegeCode, string userDesignation)
        {
            var institution = await GetInstitutionAsync(collegeCode);

            if (institution == null) return null;

            var lookupData = await GetInstitutionLookupDataAsync(institution);

            var otherColleges = await GetOtherHealthScienceCollegesAsync(collegeCode);

            var documents = await GetInstitutionDocumentsAsync();

            var pageContext = await _verificationPageService .GetPageContextAsync(collegeCode);

            var verification = await _verificationService.GetVerificationAsync<AffInstitutionsDetail>(x => x.CollegeCode == collegeCode, userDesignation);

            var feedback = await _verificationPageService.GetTabSectionFeedbackAsync(collegeCode, 1);

            return new InstitutionDetailsVerificationVm
            {
                PageContext = pageContext,
                Institution = institution,

                TypeOfInstitutionText = lookupData.TypeOfInstitution,
                StatusOfCollegeText = lookupData.StatusOfCollege,
                TalukText = lookupData.Taluk,
                DistrictText = lookupData.District,

                MemberOfGovBodyDocumentId = documents.MemberOfGovBodyDocumentId,

                AppointmentOrderDocumentId = documents.AppointmentOrderDocumentId,

                GovAutonomousDocumentId = documents.GovAutonomousDocumentId,
                OtherHealthScienceColleges = otherColleges,
                SectionFeedback = feedback,
                Verification = verification
            };
        }
        private async Task<InstitutionLookupData> GetInstitutionLookupDataAsync(AffInstitutionsDetail institution)
        {
            var result = new InstitutionLookupData
            {
                TypeOfInstitution = institution.TypeOfInstitution,
                StatusOfCollege = institution.StatusOfCollege,
                Taluk = institution.Taluk,
                District = institution.District
            };

            if (int.TryParse(institution.TypeOfInstitution, out int institutionTypeId))
            {
                result.TypeOfInstitution =
                    await _context.MstInstitutionTypes
                        .AsNoTracking()
                        .Where(x =>
                            x.InstitutionTypeId == institutionTypeId)
                        .Select(x => x.InstitutionType)
                        .FirstOrDefaultAsync()
                    ?? institution.TypeOfInstitution;
            }

            if (byte.TryParse(institution.StatusOfCollege, out byte statusId))
            {
                result.StatusOfCollege =
                    await _context.AffInstitutionStatusMasters
                        .AsNoTracking()
                        .Where(x =>
                            x.InstitutionStatusId == statusId &&
                            x.IsActive)
                        .Select(x => x.StatusName)
                        .FirstOrDefaultAsync()
                    ?? institution.StatusOfCollege;
            }

            if (!string.IsNullOrWhiteSpace(institution.Taluk))
            {
                result.Taluk =
                    await _context.TalukMasters
                        .AsNoTracking()
                        .Where(x =>
                            x.TalukId == institution.Taluk)
                        .Select(x => x.TalukName)
                        .FirstOrDefaultAsync()
                    ?? institution.Taluk;
            }

            if (!string.IsNullOrWhiteSpace(institution.District))
            {
                result.District =
                    await _context.DistrictMasters
                        .AsNoTracking()
                        .Where(x =>
                            x.DistrictId == institution.District)
                        .Select(x => x.DistrictName)
                        .FirstOrDefaultAsync()
                    ?? institution.District;
            }

            return result;
        }

        private async Task<List<OtherHealthScienceCollegeVm>> GetOtherHealthScienceCollegesAsync(string collegeCode)
        {
            return await _context.OtherHealthScienceColleges
                .AsNoTracking()
                .Where(x => x.CollegeCode == collegeCode)
                .Join(
                    _context.MstCourses,
                    ohs => ohs.CourseCode,
                    course => course.CourseCode,
                    (ohs, course) =>
                        new OtherHealthScienceCollegeVm
                        {
                            OtherCollegeCode = ohs.OtherCollegeCode,
                            CollegeName =
                                ohs.OtherCollegeCodeNavigation.CollegeName,
                            FacultyName =
                                ohs.Faculty.FacultyName,
                            CourseCode = course.CourseCode,
                            CourseName = course.CourseName
                        })
                .ToListAsync();
        }

        private async Task<InstitutionDocumentIds> GetInstitutionDocumentsAsync()
        {
            var documentIds = await _context.MstDocuments
                .AsNoTracking()
                .Where(d =>
                    d.DocumentName == "Members of Governing Body or Council" ||
                    d.DocumentName ==  "Appointment Order of Dean" ||
                    d.DocumentName == "Government Autonomous Certificate")
                .ToDictionaryAsync(
                    d => d.DocumentName,
                    d => d.DocumentId);

            documentIds.TryGetValue( "Members of Governing Body or Council", out var memberOfGovBody);

            documentIds.TryGetValue( "Appointment Order of Dean", out var appointmentOrder);

            documentIds.TryGetValue( "Government Autonomous Certificate", out var govAutonomous);

            return new InstitutionDocumentIds
            {
                MemberOfGovBodyDocumentId = memberOfGovBody == 0 ? null : memberOfGovBody,

                AppointmentOrderDocumentId = appointmentOrder == 0 ? null : appointmentOrder,

                GovAutonomousDocumentId = govAutonomous == 0 ? null : govAutonomous
            };
        }

        private async Task<AffInstitutionsDetail?> GetInstitutionAsync( string collegeCode)
        {
            return await _context.AffInstitutionsDetails.AsNoTracking().FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);
        }
    }
}