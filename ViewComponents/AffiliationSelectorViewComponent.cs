using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VerificationPortal.DATA;
using VerificationPortal.Models;

namespace VerificationPortal.ViewComponents
{
    public class AffiliationSelectorViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AffiliationSelectorViewComponent(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new AffiliationSelectorVM();

            // 1. Get logged-in user ID
            var userIdClaim = HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return View(model);
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return View(model);
            }

            // 2. Get user
            var user = await _context.TblRguhsFacultyUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return View(model);
            }

            // 3. Check assigned colleges
            var hasCollegeAssignment =
                await _context.TblCollegeMappings
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.UserId == userId &&
                        x.IsActive);

            model.HasCollegeAssignment = hasCollegeAssignment;

            if (!hasCollegeAssignment)
            {
                model.Message =
                    "No colleges have been assigned to you yet. Please contact the administrator.";

                return View(model);
            }

            // 4. Current affiliation = Continuation of Affiliation
            model.AffiliationTypes =
                await _context.TypeOfAffiliations
                    .AsNoTracking()
                    .Where(x => x.TypeId == 2)
                    .Select(x => new TypeOfAffiliationOptionVM
                    {
                        TypeId = x.TypeId,
                        TypeDescription = x.TypeDescription
                    })
                    .ToListAsync();

            model.SelectedAffiliationTypeId = 2;

            // 5. Get course levels based on faculty
            var courseLevels =
                await _context.MstCourses
                    .AsNoTracking()
                    .Where(x =>
                        x.FacultyCode == user.Faculty &&
                        !string.IsNullOrEmpty(x.CourseLevel))
                    .Select(x => x.CourseLevel)
                    .Distinct()
                    .ToListAsync();

            model.CourseLevels = courseLevels
                .Select(level => new CourseLevelOptionVM
                {
                    Level = level,

                    DisplayName = level switch
                    {
                        "UG" => "Under Graduate",
                        "PG" => "Post Graduate",
                        "SS" => "Super Speciality",
                        _ => level
                    },

                    Icon = level switch
                    {
                        "UG" => "bi-mortarboard-fill",
                        "PG" => "bi-book-half",
                        "SS" => "bi-award-fill",
                        _ => "bi-book"
                    }
                })
                .ToList();

            return View(model);
        }
    }
}