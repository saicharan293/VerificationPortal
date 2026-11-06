using System.Collections.Generic;

namespace VerificationPortal.Models
{
    public class AnimalHouseDetailsVerificationVm
    {
        // Common verification page context
        public VerificationPageContextVm PageContext { get; set; } = new();

        // Animal House details
        public List<AnimalHouseDetail> AnimalHouseDetails { get; set; } = new();

        // Section-level verification feedback
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }
}