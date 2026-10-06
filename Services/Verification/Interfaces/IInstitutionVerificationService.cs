using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification
{
    public interface IInstitutionVerificationService
    {
        Task<InstitutionDetailsVerificationVm?> GetInstitutionDetailsAsync( string collegeCode, string userDesignation);
    }
}