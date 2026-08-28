using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Models
{
    public class VerificationRequest
    {
        public string Role { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Remarks { get; set; } = string.Empty;

        public string VerifiedBy { get; set; } = string.Empty;
    }

    public class VerificationDisplayModel
    {
        public string? Remarks { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public bool? IsVerified { get; set; }

        public List<VerificationHistoryViewModel> History { get; set; } = new();
    }
}
