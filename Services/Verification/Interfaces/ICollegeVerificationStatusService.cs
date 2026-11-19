using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface ICollegeVerificationStatusService
    {
        Task<string> GetOverallVerificationStatusAsync(int facultyId, string collegeCode, string? courseLevel, string? typeOfInstitution);

        Task<List<TabSectionVerificationStatusVm>> GetTabVerificationDetailsAsync(int facultyId, string collegeCode, string? courseLevel, string? typeOfInstitution);
    }
}
