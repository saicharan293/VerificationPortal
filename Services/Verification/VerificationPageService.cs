using Azure.Core;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace VerificationPortal.Services.Verification
{
    public class VerificationPageService : IVerificationPageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VerificationPageService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) 
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<SectionFeedbackViewModel>> GetSectionFeedbackAsync( string collegeCode, int tabId, string? returnUrl = null)
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

        private bool IsAdminUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var isAdminClaim = user?.FindFirst("IsAdmin");
            return isAdminClaim != null &&  bool.TryParse(isAdminClaim.Value, out bool isAdmin) && isAdmin;
        }

        public async Task<List<SectionFeedbackViewModel>> GetSectionFeedbackByTabNameAsync(string collegeCode, string tabName)
        {
            if (IsAdminUser())
                return new List<SectionFeedbackViewModel>();

            var httpContext = _httpContextAccessor.HttpContext;

            var courseLevel = httpContext.Session.GetString("CourseLevel");

            var tab = await _context.MstTabs.AsNoTracking().FirstOrDefaultAsync(e => e.TabName == tabName);

            if (tab == null)
                return new List<SectionFeedbackViewModel>();

            var sections = await _context.MstSections
                .AsNoTracking()
                .Where(e => e.TabId == tab.TabId)
                .OrderBy(e => e.SectionId)
                .ToListAsync();

            var feedback = await _context.SectionWiseFeedbacks
                .AsNoTracking()
                .Where(e =>
                    e.CollegeCode == collegeCode &&
                    e.CourseLevel == courseLevel &&
                    e.TabId == tab.TabId
                ).ToListAsync();

            var returnUrl = httpContext == null ? string.Empty : $"{httpContext.Request.Path}{httpContext.Request.QueryString}";

            return sections.Select(section =>
            {
                var sectionFeeback = feedback.FirstOrDefault(e => e.SectionId == section.SectionId);

                return new SectionFeedbackViewModel
                {
                    FacultyId = sectionFeeback?.FacultyId ?? 2,
                    CollegeCode = collegeCode,
                    TabId = tab.TabId,
                    TabName = tab.TabName,
                    SectionId = section.SectionId,
                    CourseLevel = courseLevel,
                    SectionName = section.SectionName,
                    VerificationStatus = sectionFeeback?.VerificationStatus,
                    Remarks = sectionFeeback?.Remarks,
                    VerifiedBy = sectionFeeback?.VerifiedBy,
                    VerifiedOn = sectionFeeback?.VerifiedOn,
                    IsSaved = sectionFeeback != null,
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
