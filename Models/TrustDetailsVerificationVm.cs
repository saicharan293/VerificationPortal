using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Models
{
    public class TrustDetailsVerificationVm
    {
        public InstitutionBasicDetail Institution { get; set; } = new();

        public VerificationPageContextVm PageContext { get; set; } = new();

        public Dictionary<string, int> Documents { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public VerificationDisplayModel? Verification { get; set; }

        public TabWiseVerificationResultVm TabwiseVerification { get; set; } = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }
}
