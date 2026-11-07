using VerificationPortal.DTOs;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IInstitutionDetailsService
    {
        Task<InstitutionDetailsPageData?> GetInstitutionDetailsAsync(string collegeCode);
    }
}
