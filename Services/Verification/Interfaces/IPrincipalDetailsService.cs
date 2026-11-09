using Microsoft.AspNetCore.Mvc.Rendering;
using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IPrincipalDetailsService
    {
        Task<PrincipalDetailsViewModel?> GetPrincipalDetailsAsync(string collegecode, List<SelectListItem> experiencedColleges);
    }
}
