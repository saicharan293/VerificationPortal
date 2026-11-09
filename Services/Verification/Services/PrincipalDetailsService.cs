using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class PrincipalDetailsService : IPrincipalDetailsService
    {
        private readonly ApplicationDbContext _context;
        
        public PrincipalDetailsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PrincipalDetailsViewModel> GetPrincipalDetailsAsync(string collegeCode, List<SelectListItem> experienceColleges)
        {
            var institution = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            if (institution == null)
                return null;

            var facultyCode = institution.FacultyCode;

            // COLLEGE

            var college = await _context.AffiliationCollegeMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CollegeCode == collegeCode);

            // PRINCIPAL

            var principal = await _context.AffPrincipalDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyCode);

            if (principal == null)
                return null;
            // QUALIFICATION NAME

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

            // RECOGNITION

            string recognitionLabel = "Recognized by DCI";
            string recognitionStatus = "-";

            if (facultyCode == "2")
            {
                recognitionLabel = "Recognized by DCI";

                recognitionStatus = principal.RecognizedByDci == true
                    ? "Yes"
                    : principal.RecognizedByDci == false
                        ? "No"
                        : "-";
            }
            else if (facultyCode == "1")
            {
                recognitionLabel = "Recognized by MCI";

                recognitionStatus = principal.RecognizedByMci == true
                    ? "Yes"
                    : principal.RecognizedByMci == false
                        ? "No"
                        : "-";
            }

            // TEACHING EXPERIENCE

            var teachingExperiences =
                await _context.AffPrincipalTeachingExperiences
                    .AsNoTracking()
                    .Where(t => t.DeanId == principal.Id)
                    .OrderBy(t => t.Id)
                    .ToListAsync();

            // ADMINISTRATIVE EXPERIENCE

            var administrativeExperiences =
                await _context.AffPrincipalAdministrativeExperiences
                    .AsNoTracking()
                    .Where(a => a.DeanId == principal.Id)
                    .OrderBy(a => a.Id)
                    .ToListAsync();

            var model = new PrincipalDetailsViewModel
            {
                Institution = institution,
                CollegeName = college?.CollegeName ?? "Unknown college",
                PrincipalName = principal.DeanOrDirectorName ?? "-",
                PrincipalQualification = principalQualification,
                PrincipalQualificationDate = principal.DeanQualificationDate?.ToString("dd-MM-yyyy") ?? "-",
                RecognitionLabel = recognitionLabel,
                RecognitionStatus = recognitionStatus,
                TeachingExperiences = teachingExperiences,
                AdministrativeExperiences = administrativeExperiences,
                ExperienceColleges = experienceColleges ?? new()
            };

            return model;
        }
    }
}
