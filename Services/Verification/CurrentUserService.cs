using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContextAccessor = httpContextAccessor;    
        }

        public bool IsAdmin()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var isAdminClaim = user?.FindFirst("IsAdmin");

            return isAdminClaim != null && bool.TryParse(isAdminClaim.Value, out bool isAdmin) && isAdmin;
        }

        public string? GetDesignation()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst("Designation")?.Value;
        }

    }
}
