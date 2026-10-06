using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IVerificationPageService
    {
        Task<VerificationPageContext> GetPageContextAsync(string collegeCode);

        Task<List<SectionFeedbackViewModel>> GetTabSectionFeedbackAsync(string collegeCode, int tabId, string? returnUrl = null);
    }
}
