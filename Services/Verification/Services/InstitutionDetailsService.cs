using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.DTOs;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class InstitutionDetailsService : IInstitutionDetailsService
    {
        private readonly ApplicationDbContext _context;
        public InstitutionDetailsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InstitutionDetailsPageData?> GetInstitutionDetailsAsync(string collegeCode)
        {
            var college = await _context.AffiliationCollegeMasters.AsNoTracking().FirstOrDefaultAsync(e => e.CollegeCode == collegeCode);

            var institution = await _context.AffInstitutionsDetails.AsNoTracking().FirstOrDefaultAsync(e => e.CollegeCode == collegeCode);

            if(institution == null) return null;

            string typeOfInstitution = institution.TypeOfInstitution;

            if (int.TryParse(institution.TypeOfInstitution, out int institutionTypeId))
            {
                typeOfInstitution =
                    await _context.MstInstitutionTypes
                        .AsNoTracking()
                        .Where(x => x.InstitutionTypeId == institutionTypeId)
                        .Select(x => x.InstitutionType)
                        .FirstOrDefaultAsync()
                    ?? institution.TypeOfInstitution;
            }

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

            string taluk = institution.Taluk;

            if (!string.IsNullOrWhiteSpace(institution.Taluk))
            {
                taluk =
                    await _context.TalukMasters
                        .AsNoTracking()
                        .Where(x => x.TalukId == institution.Taluk)
                        .Select(x => x.TalukName)
                        .FirstOrDefaultAsync()
                    ?? institution.Taluk;
            }

            string district = institution.District;

            if (!string.IsNullOrWhiteSpace(institution.District))
            {
                district =
                    await _context.DistrictMasters
                        .AsNoTracking()
                        .Where(x => x.DistrictId == institution.District)
                        .Select(x => x.DistrictName)
                        .FirstOrDefaultAsync()
                    ?? institution.District;
            }


            var otherHealthScienceColleges = await _context.OtherHealthScienceColleges
                .AsNoTracking()
                .Where(x => x.CollegeCode == collegeCode)
                .Join(
                    _context.MstCourses.AsNoTracking(),
                    ohs => ohs.CourseCode,
                    course => course.CourseCode,
                    (ohs, course) => new OtherHealthScienceCollegeVm
                    {
                        OtherCollegeCode = ohs.OtherCollegeCode,
                        CollegeName = ohs.OtherCollegeCodeNavigation.CollegeName,
                        FacultyName = ohs.Faculty.FacultyName,
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName
                    })
                .ToListAsync();

            var documentNames = new List<string>
            {
                "Appointment Order of Dean",
                "Government Autonomous Certificate"
            };

            if (institution.FacultyCode == "2")
            {
                documentNames.Add("Members of Governing Body or Council");
            }

            var documentIds = await _context.MstDocuments
                .AsNoTracking()
                .Where(x =>  x.FacultyId.ToString() == institution.FacultyCode && documentNames.Contains(x.DocumentName))
                .ToDictionaryAsync(
                    x => x.DocumentName,
                    x => x.DocumentId);

            documentIds.TryGetValue( "Members of Governing Body or Council", out var memberOfGovBodyDocumentId);

            documentIds.TryGetValue( "Appointment Order of Dean", out var deanAppointmentOrderDocumentId);

            documentIds.TryGetValue( "Government Autonomous Certificate", out var govAutonomousCertificateId);

            var vm = new InstitutionDetailsVerificationVm
            {
                Institution = institution,

                TypeOfInstitutionText = typeOfInstitution,

                StatusOfCollegeText = statusOfCollege,

                TalukText = taluk,

                DistrictText = district,

                MemberOfGovBodyDocumentId = memberOfGovBodyDocumentId,

                AppointmentOrderDocumentId = deanAppointmentOrderDocumentId,

                GovAutonomousDocumentId = govAutonomousCertificateId,

                OtherHealthScienceColleges = otherHealthScienceColleges,

                HasTrustDetails = institution.TypeOfInstitution != "11"
            };

            return new InstitutionDetailsPageData
            {
                CollegeName = college?.CollegeName ?? "Unknown College",
                ViewModel = vm
            };
        }
    }
}
