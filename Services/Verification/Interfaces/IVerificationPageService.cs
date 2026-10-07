using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IVerificationPageService
    {
        Task<List<SectionFeedbackViewModel>> GetTabSectionFeedbackAsync( string collegeCode, int tabId, string? returnUrl = null);

        Task<int?> GetDocumentIdAsync(string documentName);
    }
}
