using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.ViewModels
{
    public class PgCourseDetailsViewModel
    {
        // PG / Super Specialty Course Details
        public List<AffiliationPgSsCourseDetail> Courses { get; set; } = new();

        // Page Context
        public VerificationPageContextVm PageContext { get; set; } = new();

        // Tab-wise Verification
        public TabWiseVerificationResultVm TabwiseVerification { get; set; } = new();

        // Section Feedback
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();

        // Verification Details
        public VerificationDisplayModel? Verification { get; set; }
    }
}