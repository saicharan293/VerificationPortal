using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface ISectionFeedbackService
    {
        Task<List<SectionFeedbackViewModel>> GetSectionFeedbackByTabNameAsync(string collegeCode, string tabName, int facultyCode, string? courseLevel, string? currentUser, bool isAdmin, string returnUrl);
    }
}
