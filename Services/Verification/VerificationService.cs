using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using VerificationPortal.DATA;
using VerificationPortal.Services.Verification.Interfaces;
using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Services.Verification
{
    public class VerificationService : IVerificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _provider;

        public VerificationService( ApplicationDbContext context, IServiceProvider provider)
        {
            _context = context;
            _provider = provider;
        }

        public async Task SaveVerificationAsync<T>(
            Expression<Func<T, bool>> predicate,
            VerificationRequest request)
            where T : class
        {
            // Get the record
            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(predicate);

            if (entity == null)
                throw new Exception($"{typeof(T).Name} record not found.");

            // Get the corresponding handler
            var handler = _provider.GetRequiredService<IVerificationHandler<T>>();

            // Apply verification
            handler.ApplyVerification(entity, request);

            // Save changes
            await _context.SaveChangesAsync();
        }
       
        public async Task<VerificationDisplayModel> GetVerificationAsync<T>( Expression<Func<T, bool>> predicate, string role)  where T : class
        {
            var entity = await _context.Set<T>()
                .FirstOrDefaultAsync(predicate);

            if (entity == null)
                throw new Exception("{typeof(T).Name} record not found.");

            var handler =
                _provider.GetRequiredService<IVerificationHandler<T>>();

            return handler.GetVerification(entity, role);
        }
    }
}