using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IVerificationPageService
    {
        Task<List<SectionFeedbackViewModel>> GetSectionFeedbackAsync( string collegeCode, int tabId, string? returnUrl = null);
        Task<List<SectionFeedbackViewModel>> GetSectionFeedbackByTabNameAsync(string collegeCode, string tabName);
        Task<int?> GetDocumentIdAsync(string documentName);
    }
}
