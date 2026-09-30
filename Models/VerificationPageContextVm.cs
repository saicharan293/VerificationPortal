namespace VerificationPortal.Models
{
    public class VerificationPageContextVm
    {
        public int FacultyId { get; set; }
        public string? CollegeCode { get; set; }
        public string? CollegeName { get; set; }
        public string? InstitutionName { get; set; }
        public string? FacultyName { get; set; }

        public string? CurrentVerifier { get; set; }

        public string VerificationStatus { get; set; } = "Pending";
        public string StatusBadgeClass { get; set; } = "pending";

        public string? PrevTabAction { get; set; }
        public string? PrevTabLabel { get; set; }

        public string? NextTabAction { get; set; }
        public string? NextTabLabel { get; set; }
    }
}
