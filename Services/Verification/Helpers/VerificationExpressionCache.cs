using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using VerificationPortal.Services.Verification.Constants;

namespace VerificationPortal.Services.Verification.Helpers
{
    public static class VerificationExpressionCache<T> where T : class
    {
        private static readonly ConcurrentDictionary<string, VerificationAccessor<T>> _cache
            = new();

        public static VerificationAccessor<T> Get(string role)
        {
            return _cache.GetOrAdd(role, CreateAccessor);
        }

        private static Func<T, TProperty?> CreateGetter<TProperty>(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new InvalidOperationException(
                    $"Property '{propertyName}' not found on type '{typeof(T).Name}'.");

            var entity = Expression.Parameter(typeof(T), "entity");
            var propertyAccess = Expression.Property(entity, property);

            var lambda = Expression.Lambda<Func<T, TProperty?>>(
                Expression.Convert(propertyAccess, typeof(TProperty)),
                entity);

            return lambda.Compile();
        }

        private static Action<T, TProperty?> CreateSetter<TProperty>(string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new InvalidOperationException( $"Property '{propertyName}' not found on type '{typeof(T).Name}'.");

            if (!property.CanWrite)
                throw new InvalidOperationException( $"Property '{propertyName}' does not have a setter.");

            var entity = Expression.Parameter(typeof(T), "entity");
            var value = Expression.Parameter(typeof(TProperty), "value");

            var propertyAccess = Expression.Property(entity, property);

            var assign = Expression.Assign( propertyAccess, Expression.Convert(value, property.PropertyType));

            var lambda = Expression.Lambda<Action<T, TProperty?>>( assign,  entity, value);

            return lambda.Compile();
        }

        private static VerificationAccessor<T> CreateAccessor(string role)
        {
            var prefix = GetPrefix(role);

            return new VerificationAccessor<T>
            {
                SetVerified = CreateSetter<bool?>($"Is{prefix}Verified"),
                SetRemarks = CreateSetter<string?>($"{prefix}Remarks"),
                SetVerifiedDate = CreateSetter<DateTime?>($"{prefix}VerifiedDate"),
                SetVerifiedBy = CreateSetter<string?>($"{prefix}Name"),

                GetVerified = CreateGetter<bool?>($"Is{prefix}Verified"),
                GetRemarks = CreateGetter<string?>($"{prefix}Remarks"),
                GetVerifiedDate = CreateGetter<DateTime?>($"{prefix}VerifiedDate"),
                GetVerifiedBy = CreateGetter<string?>($"{prefix}Name")
            };
        }

        private static string GetPrefix(string role)
        {
            return role switch
            {
                UserRoles.DataEntryOperator => "Deo",
                UserRoles.JuniorAssistant => "Jr",
                UserRoles.SectionOfficer => "So",
                UserRoles.AssistantRegistrar => "Ar",
                UserRoles.Registrar => "Rg",
                UserRoles.RegistrarEvaluation => "Re",
                UserRoles.Director => "Dr",
                UserRoles.ViceChancellor => "Vc",
                _ => throw new ArgumentException($"Unsupported role: {role}")
            };
        }
    }
}