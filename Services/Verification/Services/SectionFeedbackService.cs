using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class SectionFeedbackService : ISectionFeedbackService
    {
        private readonly ApplicationDbContext _context;

        public SectionFeedbackService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SectionFeedbackViewModel>> GetSectionFeedbackByTabNameAsync (string collegeCode, string tabName, int facultyCode, string? courseLevel, string? currentUser, bool isAdmin, string returnUrl)
        {
            if (isAdmin) return new List<SectionFeedbackViewModel>();

            var tab = await _context.MstTabs
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.TabName == tabName && e.FacultyId == facultyCode);

            if(tab == null) return new List<SectionFeedbackViewModel> ();

            var sections = await _context.MstSections
                .AsNoTracking()
                .Where(e => 
                    e.TabId == tab.TabId && 
                    e.FacultyId == facultyCode)
                .ToListAsync();

            var feedback = await _context.SectionWiseFeedbacks
                .AsNoTracking()
                .Where(e => 
                    e.CollegeCode == collegeCode && 
                    e.CourseLevel == courseLevel && 
                    e.VerifiedBy == currentUser && 
                    e.TabId == tab.TabId)
                .ToListAsync();

            var feedbackLookup = feedback
                .GroupBy(e => e.SectionId)
                .ToDictionary(g => g.Key, g => g.First());

            return sections.Select(section =>
            {
                feedbackLookup.TryGetValue(
                    section.SectionId,
                    out var sectionFeedback);

                return new SectionFeedbackViewModel
                {
                    FacultyId = sectionFeedback?.FacultyId ?? facultyCode,
                    CollegeCode = collegeCode,
                    TabId = tab.TabId,
                    TabName = tab.TabName,
                    SectionId = section.SectionId,
                    CourseLevel = courseLevel,
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
    }
}
