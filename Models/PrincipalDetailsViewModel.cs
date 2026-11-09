using Microsoft.AspNetCore.Mvc.Rendering;
using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Models
{
    public class PrincipalDetailsViewModel
    {
        // Institution details
        public AffInstitutionsDetail Institution { get; set; } = null!;

        public string CollegeName { get; set; } = "-";

        // Principal basic information
        public string PrincipalName { get; set; } = "-";
        public string PrincipalQualification { get; set; } = "-";
        public string PrincipalQualificationDate { get; set; } = "-";
        public string PrincipalUniversity { get; set; } = "-";
        public string PrincipalStateCouncilNumber { get; set; } = "-";

        // Recognition
        public string RecognitionLabel { get; set; } = "-";
        public string RecognitionStatus { get; set; } = "-";

        // Experience details
        public List<AffPrincipalTeachingExperience> TeachingExperiences
        { get; set; } = new();

        public List<AffPrincipalAdministrativeExperience> AdministrativeExperiences
        { get; set; } = new();

        // Experience college dropdown/list
        public List<SelectListItem> ExperienceColleges { get; set; } = new();

        // Shared page context
        public VerificationPageContextVm PageContext { get; set; } = new();

        // Verification
        public TabWiseVerificationResultVm TabwiseVerification { get; set; } = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();

        public VerificationDisplayModel? Verification { get; set; }
    }
}
