namespace VerificationPortal.Models
{
    public class VerificationHistoryViewModel
    {
        public string Designation { get; set; } = string.Empty;

        public bool? IsVerified { get; set; }

        public string? Remarks { get; set; }

        public string? VerifiedBy { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public string Status =>
            IsVerified switch
            {
                true => "Approved",
                false => "Rejected",
                null => "Pending"
            };
    }
}
