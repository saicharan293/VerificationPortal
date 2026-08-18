using System;

namespace VerificationPortal.Models
{
    public class SectionFeedbackViewModel
    {
        public int FacultyId { get; set; }

        public string CollegeCode { get; set; } = null!;

        public int TabId { get; set; }

        public int SectionId { get; set; }

        public string SectionName { get; set; } = null!;

        public string? VerificationStatus { get; set; }

        public string? Remarks { get; set; }

        public string? VerifiedBy { get; set; }

        public DateTime? VerifiedOn { get; set; }

        public bool IsSaved { get; set; }

        public string? ReturnUrl { get; set; }
    }
}