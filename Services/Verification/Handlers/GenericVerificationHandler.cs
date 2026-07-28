using VerificationPortal.Services.Verification.Helpers;
using VerificationPortal.Services.Verification.Interfaces;
using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Services.Verification.Handlers
{
    public class GenericVerificationHandler<T> : IVerificationHandler<T>
        where T : class
    {
        public void ApplyVerification(T entity, VerificationRequest request)
        {
            var accessor = VerificationExpressionCache<T>.Get(request.Role);

            accessor.SetVerified(
                entity,
                request.Status.Equals("accept", StringComparison.OrdinalIgnoreCase));

            accessor.SetRemarks(entity, request.Remarks);
            accessor.SetVerifiedBy(entity, request.VerifiedBy);
            accessor.SetVerifiedDate(entity, DateTime.Now);
        }

        public VerificationDisplayModel GetVerification(T entity, string role)
        {
            var accessor = VerificationExpressionCache<T>.Get(role);

            return new VerificationDisplayModel
            {
                IsVerified = accessor.GetVerified(entity),
                Remarks = accessor.GetRemarks(entity),
                VerifiedBy = accessor.GetVerifiedBy(entity),
                VerifiedDate = accessor.GetVerifiedDate(entity)
            };
        }
    }
}