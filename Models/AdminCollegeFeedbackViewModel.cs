namespace VerificationPortal.ViewModels
{
    public class AdminCollegeFeedbackViewModel
    {
        public string CollegeCode { get; set; } = null!;

        public string? CollegeName { get; set; }

        public string? CollegeTown { get; set; }

        public string? FacultyCode { get; set; }

        public int TotalSections { get; set; }

        public int CompletedSections { get; set; }

        public int PendingSections { get; set; }

        public int RejectedSections { get; set; }

        public string FeedbackStatus { get; set; } = "Not Started";

        public string? VerifiedBy { get; set; }

        public DateTime? LastVerifiedOn { get; set; }
    }
}