namespace VerificationPortal.Models
{
    public class UgCourseVerificationVm
    {
        // Page Context
        public VerificationPageContextVm PageContext { get; set; } = new();
        // Feedback
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();

        // Courses
        public List<UgCourseDetailVerificationVm> Courses { get; set; }  = new();
    }


    public class UgCourseDetailVerificationVm
    {

        public int FacultyId { get; set; }
        public string? CollegeCode { get; set; }

        public int? PreviousYearNotificationDocId { get; set; }
        public int? GOKOrderDocId { get; set; }
        public int? RGUHSNotificationDocId { get; set; }
        public AffiliationCourseDetail Course { get; set; } = null!;
        public List<VerificationDocumentVm> Documents { get; set; } = new();
        public SectionFeedbackViewModel? PresentIntakeFeedback { get; set; }
        public SectionFeedbackViewModel? PreviousDetailsFeedback { get; set; }
        public SectionFeedbackViewModel? PermissionFeedback { get; set; }
        public SectionFeedbackViewModel? EcFcFeedback { get; set; }
        public SectionFeedbackViewModel? LastAffiliationFeedback { get; set; }
        public SectionFeedbackViewModel? PreviousLicFeedback { get; set; }
    }
}