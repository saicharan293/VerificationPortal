using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class AcademicIntakeService : IAcademicIntakeService
    {
        private readonly ApplicationDbContext _context;
        
        public AcademicIntakeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AcademicIntakePageViewModel1> GetDentalAcademicIntakeAsync(string collegeCode, int facultyId, string? collegeName, string courseLevel)
        {
            var model = new AcademicIntakePageViewModel1
            {
                FacultyCode = facultyId.ToString(),
                CollegeCode = collegeCode,
                FacultyId = facultyId,
                CollegeName = collegeName,
                CourseLevel = courseLevel
            };

            var intakeDetails = await _context.CollegeCourseIntakeDetails
                .AsNoTracking()
                .Where(e => e.FacultyCode == facultyId && e.CollegeCode == collegeCode)
                .ToListAsync();

            var allCourses = await _context.MstCourses
                .AsNoTracking()
                .Where(e => e.FacultyCode == facultyId)
                .ToListAsync();

            var existingIntakes = await _context.AcademicIntakes
                .AsNoTracking()
                .Where(e => e.FacultyCode == facultyId.ToString() && e.CollegeCode == collegeCode)
                .ToListAsync();

            IEnumerable<IntakeByLevelViewModel1> Project(string level)
            {
                var levelCourses = allCourses
                    .Where(e => e.CourseLevel == level)
                    .ToList();

                var existingLevelIntakes = existingIntakes
                    .Where(e => 
                        int.TryParse(e.Courses, out var courseCode) &&
                        allCourses.Any(e => 
                            e.CourseCode == courseCode && 
                            e.CourseLevel == level))
                    .ToList();

                if (intakeDetails.Any())
                {
                    return 
                        from c in levelCourses
                        join d in intakeDetails
                            on c.CourseCode.ToString() equals d.CourseCode
                            into dj

                        from detail in dj.DefaultIfEmpty()

                        join e in existingIntakes
                            on c.CourseCode.ToString() equals e.Courses
                            into ej

                       from existing in ej.DefaultIfEmpty()

                       select new IntakeByLevelViewModel1
                       {
                           Id = existing?.Id ?? 0,
                           CourseCode = c.CourseCode.ToString(),
                           CourseName = c.CourseName,
                           HasAY2025DciDocument = existing?.Ay2025Dcidocument != null && existing.Ay2025Dcidocument.Length > 0,
                           HasLopDocument = existing?.Ay2025LopDentalDocument != null && existing.Ay2025LopDentalDocument.Length > 0,

                           // AY 2025-26
                           AY2025_ExistingIntake = existing?.Ay2025ExistingIntake ?? detail?.ExistingIntake ?? 0,
                           AY2025_LopNmcIntake = existing?.Ay2025LopNmcIntake ?? 0,
                           AY2025_TotalIntake = existing?.Ay2025TotalIntake ?? 0,
                           AY2025_LopDate = existing?.Ay2025LopDate,

                           // AY 2026-27
                           AY2026_ExistingIntake = existing?.Ay2026ExistingIntake ?? 0,
                           AY2026_AddRequestedIntake = existing?.Ay2026AddRequestedIntake ?? 0,
                           AY2026_TotalIntake = existing?.Ay2026TotalIntake ?? 0,
                           HasAY2026DciDocument = existing?.Ay2026Dcidocument != null && existing.Ay2026Dcidocument.Length > 0,

                       };
                }

                if (existingLevelIntakes.Any())
                {
                    return
                        from c in levelCourses

                        join e in existingIntakes
                            on c.CourseCode.ToString() equals e.Courses
                            into ej
                        
                        from existing in ej.DefaultIfEmpty()

                        select new IntakeByLevelViewModel1
                        {
                            Id = existing?.Id ?? 0,

                            CourseCode = c.CourseCode.ToString(),
                            CourseName = c.CourseName,

                            // HasNmcDocument = existing?.Ay2025NmcDocument != null && existing.Ay2025NmcDocument.Length > 0,

                            // HasLopDocument = !string.IsNullOrWhiteSpace( existing?.Ay2025LopDentalDocument),

                            HasAY2025DciDocument = !string.IsNullOrWhiteSpace( existing?.Ay2025Dcidocument),

                            // HasAY2025KsdcDocument =!string.IsNullOrWhiteSpace(existing?.Ay2025Ksdcdocument),

                            HasAY2026DciDocument =!string.IsNullOrWhiteSpace(existing?.Ay2026Dcidocument),
                            HasAY2026KsdcDocument = !string.IsNullOrWhiteSpace( existing?.Ay2026Ksdcdocument),
                            HasAY2027DciDocument = !string.IsNullOrWhiteSpace( existing?.Ay2027Dcidocument),
                            HasAY2027KsdcDocument = !string.IsNullOrWhiteSpace( existing?.Ay2027Ksdcdocument),
                            HasLopDocument = !string.IsNullOrWhiteSpace(existing.Ay2025LopDentalDocument),



                            // AY 2025-26
                            AY2025_ExistingIntake = existing?.Ay2025ExistingIntake ?? 0,
                            AY2025_LopNmcIntake = existing?.Ay2025LopNmcIntake ?? 0,
                            AY2025_TotalIntake = existing?.Ay2025TotalIntake ?? 0,
                            AY2025_LopDate = existing?.Ay2025LopDate,

                            // AY 2026-27
                            AY2026_ExistingIntake = existing?.Ay2026ExistingIntake ?? 0,
                            AY2026_AddRequestedIntake = existing?.Ay2026AddRequestedIntake ?? 0,
                            AY2026_TotalIntake = existing?.Ay2026TotalIntake ?? 0,

                            // AY 2027-28
                            AY2027_ExistingIntake = existing?.Ay2027ExistingIntake ?? 0,
                            AY2027_AddRequestedIntake = existing?.Ay2027AddRequestedIntake ?? 0,
                            AY2027_TotalIntake = existing?.Ay2027TotalIntake ?? 0
                        };
                }

                return 
                    from c in levelCourses

                    join e in existingIntakes
                        on c.CourseCode.ToString() equals e.Courses
                        into ej
                    
                    from existing in ej.DefaultIfEmpty()

                    select new IntakeByLevelViewModel1
                    {
                        CourseCode = c.CourseCode.ToString(),
                        CourseName = c.CourseName,

                        
                        // AY 2025-26
                        AY2025_ExistingIntake = existing?.Ay2025ExistingIntake ?? 0,
                        AY2025_LopNmcIntake = existing?.Ay2025LopNmcIntake ?? 0,
                        AY2025_TotalIntake = existing?.Ay2025TotalIntake ?? 0,
                        AY2025_LopDate = existing?.Ay2025LopDate,
                        HasAY2025DciDocument = !string.IsNullOrWhiteSpace(existing.Ay2025Dcidocument),
                        HasLopDocument = !string.IsNullOrWhiteSpace(existing.Ay2025LopDentalDocument),

                        // AY 2026-27
                        AY2026_ExistingIntake = existing?.Ay2026ExistingIntake ?? 0,
                        AY2026_AddRequestedIntake = existing?.Ay2026AddRequestedIntake ?? 0,
                        AY2026_TotalIntake = existing?.Ay2026TotalIntake ?? 0,
                        HasAY2026DciDocument = !string.IsNullOrWhiteSpace(existing.Ay2026Dcidocument),

                        // AY 2027-28
                        // AY2027_ExistingIntake = existing?.Ay2027ExistingIntake ?? 0,
                        // AY2027_AddRequestedIntake = existing?.Ay2027AddRequestedIntake ?? 0,
                        // AY2027_TotalIntake = existing?.Ay2027TotalIntake ?? 0
                    };
            }

            switch (courseLevel.ToUpperInvariant())
            {
                case "UG":
                    model.UgCourses = Project("UG")
                        .DistinctBy(e => e.CourseCode)
                        .ToList();
                    break;

                case "PG":
                    model.PgCourses = Project("PG")
                        .DistinctBy(e => e.CourseCode)
                        .ToList();
                    break;

                case "SS":
                    model.SsCourses = Project("SS")
                        .DistinctBy(e => e.CourseCode)
                        .ToList();
                    break;

                default:
                    throw new ArgumentException("Invalid course level.", nameof(courseLevel));
            }

            // Populate saved intake summary.
            model.SavedIntakes =
            (
                from e in existingIntakes
                where int.TryParse(e.Courses, out _)

                let courseCode = int.Parse(e.Courses!)

                join c in allCourses
                    on courseCode equals c.CourseCode

                where c.CourseLevel == courseLevel

                select new SavedAcademicIntakeRowViewModel1
                {
                    Id = e.Id,
                    CourseLevel = c.CourseLevel,
                    CourseCode = e.Courses ?? string.Empty,
                    CourseName = c.CourseName,

                    // AY2024_TotalIntake = e.Ay2024TotalIntake,
                    
                    // AY2025_TotalIntake = e.Ay2025TotalIntake,
                    AY2026_ExistingIntake = e.Ay2026ExistingIntake,
                    AY2026_AddReqIntake = e.Ay2026AddRequestedIntake,
                    AY2026_TotalIntake = e.Ay2026TotalIntake,

                    AY2027_ExistingIntake = e.Ay2027ExistingIntake,
                    AY2027_AddReqIntake = e.Ay2027AddRequestedIntake,
                    AY2027_TotalIntake = e.Ay2027TotalIntake,
                    HasNmcDocument = e.Ay2025NmcDocument != null && e.Ay2025NmcDocument.Length > 0,
                    HasLopDocument = e.Ay2025LopDentalDocument != null && e.Ay2025LopDentalDocument.Length > 0,
                    Has2025DciDocument = !string.IsNullOrWhiteSpace(e.Ay2025Dcidocument),
                    Has2026DciDocument = !string.IsNullOrWhiteSpace(e.Ay2026Dcidocument),
                    Has2027DciDocument = !string.IsNullOrWhiteSpace(e.Ay2027Dcidocument),
                    HasKsdcDocument =
                        // !string.IsNullOrWhiteSpace(e.Ay2025Ksdcdocument) ||
                        !string.IsNullOrWhiteSpace(e.Ay2026Ksdcdocument) ||
                        !string.IsNullOrWhiteSpace(e.Ay2027Ksdcdocument),

                    CreatedOn = DateTime.Now
                }
            ).ToList();

            return model;
        }


        public async Task<int> GetSeatSlabAsync(string collegeCode)
        {
            return await _context.AcademicIntakes
                .Where(e => e.CollegeCode == collegeCode)
                .Select(e => e.Ay2026TotalIntake)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetHospitalDetailsIdAsync(string collegeCode)
        {
            return await _context.HospitalDetailsForAffiliations
                .Where(e => e.CollegeCode == collegeCode)
                .Select(e => e.HospitalDetailsId)
                .FirstOrDefaultAsync();
        }

    }
}
