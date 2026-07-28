using System.Linq.Expressions;
using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IVerificationService
    {
        Task SaveVerificationAsync<T>(  Expression<Func<T, bool>> predicate, VerificationRequest request) where T : class;

        Task<VerificationDisplayModel> GetVerificationAsync<T>( Expression<Func<T, bool>> predicate, string role) where T : class;
    }
}
