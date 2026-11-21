using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class MedicalCollegeCourseIntakeVerificationService:IMedicalCollegeCourseIntakeVerificationService
    {
        private readonly ApplicationDbContext _context;

        private static readonly string[] PreviousIntakeSlabs =
        {
            "50",
            "51-100",
            "101-150",
            "151-200",
            "201-250",
        };

        public MedicalCollegeCourseIntakeVerificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MedicalCollegeCourseIntakeVerificationVm> GetMedicalCollegeCourseIntakeAsync(string collegeCode, int facultyId, string courseLevel, int? affiliationTypeId, bool showIncreasedIntakeFields)
        {
            var level = courseLevel.Trim().ToUpperInvariant();

            var courses = await _context.MstMedicalCollegeCourseIntakes
                .AsNoTracking()
                .Where(e =>
                    e.CollCode == collegeCode &&
                    e.Facultycode == facultyId &&
                    e.UgPg != null &&
                    e.UgPg.Trim().ToUpper() == level)
                .OrderBy(e => e.Course)
                .Select(e => new MedicalCollegeCourseIntakeVerificationRowVm
                {
                    Slno = e.Slno,
                    CourseCode = e.CourseCode.ToString()!,
                    CourseName = e.Course ?? string.Empty,
                    Intake2627 = e.Intake2627.Value,
                    IncreasedIntake = e.IncreasedIntake,
                    AcademicYear = e.AcademicYear
                })
                .ToListAsync();

            var model = new MedicalCollegeCourseIntakeVerificationVm
            {
                CollegeCode = collegeCode,
                FacultyCode = facultyId,
                CourseLevel = level,
                Courses = courses,
                ShowIncreasedIntakeFields = showIncreasedIntakeFields
            };

            if (level != "UG" || !affiliationTypeId.HasValue) return model;

            var saved = await _context.MedicalCollegePreviousIntakes
                .AsNoTracking()
                .Where(e =>
                    e.FacultyCode == facultyId &&
                    e.CollegeCode == collegeCode &&
                    e.AffiliationTypeId == affiliationTypeId)
                .Select(e => new
                {
                    e.Id,
                    e.CourseCode,
                    e.IntakeSlab,
                    e.LopYear,
                    e.NmcDocumentName,
                    HasDocument = e.NmcDocument != null
                })
                .ToListAsync();

            foreach(var course in courses)
            {
                if (!int.TryParse(course.CourseCode, out var courseCode)) continue;

                var previousCourse = new PreviousIntakeCourseVerificationVm
                {
                    CourseCode = courseCode,
                    CourseName = course.CourseName
                };

                foreach(var slab in PreviousIntakeSlabs)
                {
                    var row = saved.FirstOrDefault(e => e.CourseCode == courseCode && e.IntakeSlab == slab);

                    previousCourse.Slabs.Add(
                        new PreviousIntakeSlabVerificationVm
                        {
                            Id = row?.Id ?? 0,
                            IntakeSlab = slab,
                            LopYear = row?.LopYear,
                            HasDocument = row?.HasDocument ?? false,
                            DocumentName = row?.NmcDocumentName
                        });
                }

                model.PreviousDetails.Add(previousCourse);
            }

            return model;
        }
    }
}
