namespace VerificationPortal.Models
{
    public class TabWiseVerificationResultVm
    {
        public bool IsAdmin { get; set; }
        public bool HasFeedback { get; set; }
        public string? ExistingRemarks { get; set; }
        public string ExistingStatus { get; set; } = "Pending";
        public string ExistingStatusClass { get; set; } = "bg-warning";
        public string? VerifiedBy { get; set; }
        public string? VerifiedDate { get; set; }
        public bool ShowFeedbackForm { get; set; }
        public int? TabId { get; set; }
        public string? TabName { get; set; }
        public List<VerificationHistoryViewModel> VerificationHistory { get; set; } = new();
    }
}
