using Microsoft.AspNetCore.Mvc.Rendering;
using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Models
{
    public class DeanDirectorDetailsPageVm
    {
        public DeanDetailsViewModel DeanDetails { get; set; } = new();
        public VerificationPageContextVm PageContext { get; set; } = new();
        public List<SelectListItem> Qualifications { get; set; } = new(); 
        public List<SelectListItem> ExperienceColleges { get; set; } = new();
        public string DeanQualificationName { get; set; } = "-";
        public TabWiseVerificationResultVm TabwiseVerification { get; set; } = new();
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
        public VerificationDisplayModel? verification { get; set; }
    }
}
