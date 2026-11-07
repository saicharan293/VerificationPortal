using VerificationPortal.Models;

namespace VerificationPortal.DTOs
{
    public class InstitutionDetailsPageData
    {
        public string CollegeName { get; set; } = "Unknown College";
        public InstitutionDetailsVerificationVm ViewModel { get; set; } = new();
    }
}
