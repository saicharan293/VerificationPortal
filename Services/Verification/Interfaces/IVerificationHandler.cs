using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IVerificationHandler<T> where T : class
    {
        void ApplyVerification( T entity,  VerificationRequest request);
        VerificationDisplayModel GetVerification(T entity, string role);
    }
}
