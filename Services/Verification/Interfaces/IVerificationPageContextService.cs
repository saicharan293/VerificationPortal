using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IVerificationPageContextService
    {
        Task<VerificationPageContext> GetPageContextAsync(string collegeCode, string userId);
    }
}
