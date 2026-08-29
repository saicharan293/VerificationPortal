using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using VerificationPortal.DATA;
using VerificationPortal.Exceptions;
using VerificationPortal.Models;
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
            {
                throw new VerificationDataNotFoundException(
                    typeof(T).Name
                );
            }

            var handler =
                _provider.GetRequiredService<IVerificationHandler<T>>();

            return handler.GetVerification(entity, role);
        }


        public async Task<List<VerificationHistoryViewModel>> GetVerificationHistoryAsync<T>( Expression<Func<T, bool>> predicate) where T : class
        {
            var entity = await _context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate);

            if (entity == null)
                throw new Exception($"{typeof(T).Name} record not found.");

            var result = new List<VerificationHistoryViewModel>();

            var properties = typeof(T).GetProperties();

            // --------------------------------------------
            // DEO
            // --------------------------------------------

            AddHistory(
                result,
                properties,
                entity,
                "DEO",
                "IsDeoVerified",
                "DeoRemarks",
                "DeoVerifiedDate",
                "DeoName");

            // --------------------------------------------
            // JR
            // --------------------------------------------

            AddHistory(
                result,
                properties,
                entity,
                "JR",
                "IsJrVerified",
                "JrRemarks",
                "JrVerifiedDate",
                "JrName");

            // --------------------------------------------
            // SO
            // --------------------------------------------

            AddHistory(
                result,
                properties,
                entity,
                "SO",
                "IsSoVerified",
                "SoRemarks",
                "SoVerifiedDate",
                "SoName");

            // --------------------------------------------
            // AR
            // --------------------------------------------

            AddHistory(
                result,
                properties,
                entity,
                "AR",
                "IsArVerified",
                "ArRemarks",
                "ArVerifiedDate",
                "ArName");

            // --------------------------------------------
            // RG
            // --------------------------------------------

            AddHistory(
                result,
                properties,
                entity,
                "RG",
                "IsRgVerified",
                "RgRemarks",
                "RgVerifiedDate",
                "RgName");

            // --------------------------------------------
            // RE
            // --------------------------------------------

            AddHistory(
                result,
                properties,
                entity,
                "RE",
                "IsReVerified",
                "ReRemarks",
                "ReVerifiedDate",
                "ReName");

            // --------------------------------------------
            // DR
            // --------------------------------------------

            AddHistory(
                result,
                properties,
                entity,
                "DR",
                "IsDrVerified",
                "DrRemarks",
                "DrVerifiedDate",
                "DrName");

            // --------------------------------------------
            // VC
            // --------------------------------------------

            AddHistory(
                result,
                properties,
                entity,
                "VC",
                "IsVcVerified",
                "VcRemarks",
                "VcVerifiedDate",
                "VcName");

            return result;
        }



        private static void AddHistory<T>(
    List<VerificationHistoryViewModel> result,
    PropertyInfo[] properties,
    T entity,
    string role,
    string statusProperty,
    string remarksProperty,
    string dateProperty,
    string verifiedByProperty)
    where T : class
        {
            var statusProp = properties
                .FirstOrDefault(x => x.Name == statusProperty);

            var remarksProp = properties
                .FirstOrDefault(x => x.Name == remarksProperty);

            var dateProp = properties
                .FirstOrDefault(x => x.Name == dateProperty);

            var verifiedByProp = properties
                .FirstOrDefault(x => x.Name == verifiedByProperty);

            if (statusProp == null)
                return;

            var status = statusProp.GetValue(entity) as bool?;

            var remarks = remarksProp?.GetValue(entity) as string;

            var verifiedBy = verifiedByProp?.GetValue(entity) as string;

            var verifiedDate = dateProp?.GetValue(entity) as DateTime?;

            // Don't display users who haven't verified yet
            if (status == null &&
                string.IsNullOrWhiteSpace(remarks) &&
                string.IsNullOrWhiteSpace(verifiedBy) &&
                verifiedDate == null)
            {
                return;
            }

            result.Add(new VerificationHistoryViewModel
            {
                Designation = role,
                IsVerified = status,
                Remarks = remarks,
                VerifiedBy = verifiedBy,
                VerifiedDate = verifiedDate
            });
        }
    }
}