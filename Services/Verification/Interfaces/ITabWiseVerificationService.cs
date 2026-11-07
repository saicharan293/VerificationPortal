using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface ITabWiseVerificationService
    {
        Task<TabWiseVerificationResultVm> GetTabFeedbackByTabNameAsync(string collegeCode, string tabName, string currentUser, int? facultyId, string? courseLevel, bool isAdmin);
    }
}
