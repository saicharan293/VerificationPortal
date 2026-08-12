namespace VerificationPortal.Models
{
    public class ResearchPublicationsVerificationViewModel
    {
        public string? CollegeCode { get; set; }

        public int FacultyCode { get; set; }

        public int AffiliationType { get; set; }

        public string? CourseLevel { get; set; }

        public int? PublicationsNo { get; set; }

        public string? PublicationsPdfName { get; set; }

        public string? ClinicalTrialsPdfName { get; set; }

        public int? StudentsRGUHSFunded { get; set; }

        public int? StudentsExternalBodyFunding { get; set; }

        public string? StudentsProjectsPdfName { get; set; }

        public int? FacultyRGUHSFunded { get; set; }

        public int? FacultyExternalBodyFunding { get; set; }

        public string? FacultyProjectsPdfName { get; set; }

        public List<DepartmentWisePublicationVerificationRow>
            DepartmentWisePublications
        { get; set; } = new();
    }


    public class DepartmentWisePublicationVerificationRow
    {
        public int Id { get; set; }

        public string? DeptCode { get; set; }

        public string? DeptName { get; set; }

        public int PublicationsCount { get; set; }

        public string? PublicationPath { get; set; }
    }
}
