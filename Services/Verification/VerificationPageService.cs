using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification
{
    public class VerificationPageService : IVerificationPageService
    {
        private readonly ApplicationDbContext _context;

        public VerificationPageService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<List<SectionFeedbackViewModel>> GetTabSectionFeedbackAsync( string collegeCode, int tabId, string? returnUrl = null)
        {
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

                    VerificationStatus =
                        sectionFeedback?.VerificationStatus,

                    Remarks =
                        sectionFeedback?.Remarks,

                    VerifiedBy =
                        sectionFeedback?.VerifiedBy,

                    VerifiedOn =
                        sectionFeedback?.VerifiedOn,

                    IsSaved = sectionFeedback != null,

                    ReturnUrl = returnUrl
                };
            }).ToList();
        }

        public async Task<int?> GetDocumentIdAsync(string documentName)
        {
            if (string.IsNullOrWhiteSpace(documentName)) return null;

            return await _context.MstDocuments.AsNoTracking().Where(d => d.DocumentName == documentName)
                .Select(e => (int?)e.DocumentId)
                .FirstOrDefaultAsync();
        }
    }
}
