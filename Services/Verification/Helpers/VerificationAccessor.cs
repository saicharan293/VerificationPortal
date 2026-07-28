namespace VerificationPortal.Services.Verification.Helpers
{
    public class VerificationAccessor<T>
    {
        public Action<T, bool?> SetVerified { get; init; } = default!;
        public Action<T, string?> SetRemarks { get; init; } = default!;
        public Action<T, DateTime?> SetVerifiedDate { get; init; } = default!;
        public Action<T, string?> SetVerifiedBy { get; init; } = default!;

        public Func<T, bool?> GetVerified { get; init; } = default!;
        public Func<T, string?> GetRemarks { get; init; } = default!;
        public Func<T, DateTime?> GetVerifiedDate { get; init; } = default!;
        public Func<T, string?> GetVerifiedBy { get; init; } = default!;
    }
}