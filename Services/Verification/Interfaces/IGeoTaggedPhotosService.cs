using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IGeoTaggedPhotosService
    {
        Task<GeoTaggedPhotosVm> GetPhotosAsync(string collegeCode, string facultyCode);
    }
}
