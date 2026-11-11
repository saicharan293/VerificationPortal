using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IAcademicIntakeService
    {
        Task<AcademicIntakePageViewModel1> GetDentalAcademicIntakeAsync(string collegeCode, int facultyId, string? collegeName, string courseLevel);
        Task<int> GetSeatSlabAsync(string collegeCode);
        Task<int> GetHospitalDetailsIdAsync(string collegeCode);
    }
}
