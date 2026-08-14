namespace VerificationPortal.Models
{
    public class InstitutionDetailsVerificationVm
    {
        public AffInstitutionsDetail Institution { get; set; } = null!;

        public string? TypeOfInstitutionText { get; set; }
        public string? StatusOfCollegeText { get; set; }
        public string? TalukText { get; set; }
        public string? DistrictText { get; set; }
    }
}
