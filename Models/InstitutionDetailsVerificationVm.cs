namespace VerificationPortal.Models
{
    public class InstitutionDetailsVerificationVm
    {
        public VerificationPageContext PageContext { get; set; } = new();
        public AffInstitutionsDetail Institution { get; set; } = null!;
        public string? TypeOfInstitutionText { get; set; }
        public string? StatusOfCollegeText { get; set; }
        public string? TalukText { get; set; }
        public string? DistrictText { get; set; }

        public int? MemberOfGovBodyDocumentId { get; set; }
        public int? AppointmentOrderDocumentId { get; set; }
        public int? GovAutonomousDocumentId { get; set; }

        // Managing Other Health Science Colleges
        public List<OtherHealthScienceCollegeVm> OtherHealthScienceColleges { get; set; }
            = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; }
            = new();
    }

    public class OtherHealthScienceCollegeVm
    {
        public string? OtherCollegeCode { get; set; }

        public string? CollegeName { get; set; }

        public string? FacultyName { get; set; }

        public int CourseCode { get; set; }
        public string CourseName { get; set; }
    }
}