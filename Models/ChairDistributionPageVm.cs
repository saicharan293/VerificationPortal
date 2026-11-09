using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Models
{
    public class ChairDistributionPageVm
    {
        public List<DentalChairVm> Chairs { get; set; } = new();

        public VerificationPageContextVm PageContext { get; set; } = new();

        public TabWiseVerificationResultVm TabwiseVerification { get; set; } = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();

        public VerificationDisplayModel? Verification { get; set; }
    }
}