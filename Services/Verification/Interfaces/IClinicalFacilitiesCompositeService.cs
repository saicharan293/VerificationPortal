using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IClinicalFacilitiesCompositeService
    {
        Task<ClinicalFacilitiesVerificationViewModel>
            GetClinicalFacilitiesAsync(
                string collegeCode,
                VerificationPageContext pageContext);
    }
}
