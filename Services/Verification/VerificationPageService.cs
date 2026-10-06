using Azure.Core;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification
{
    public class VerificationPageService : IVerificationPageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public VerificationPageService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<VerificationPageContext> GetPageContextAsync(string collegeCode)
        {
            var institution = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CollegeCode == collegeCode);

            if (institution == null)
            {
                throw new Exception(
                    $"Institution not found for college code '{collegeCode}'.");
            }

            if (string.IsNullOrWhiteSpace(institution.FacultyCode))
            {
                throw new Exception("Faculty code not found.");
            }

            var faculty = await _context.Faculties
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.FacultyId.ToString() == institution.FacultyCode);

            return new VerificationPageContext
            {
                Institution = institution,
                Faculty = faculty
            };
        }

        public async Task<List<SectionFeedbackViewModel>> GetTabSectionFeedbackAsync(string collegeCode, int tabId, string? returnURl = null)
        {
            // Admin users don't need section-wise feedback - they only use page-wise verification
            if (_currentUserService.IsAdmin())
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
                    ReturnUrl = returnURl
                };
            }).ToList();
        }
    }
}
