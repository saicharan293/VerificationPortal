using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IMedicalCollegeCourseIntakeVerificationService
    {
        Task<MedicalCollegeCourseIntakeVerificationVm> GetMedicalCollegeCourseIntakeAsync(string collegeCode, int facultyId, string courseLevel, int? affiliationTypeId, bool showIncreasedIntakeFields);

    }
}
