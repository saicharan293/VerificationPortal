namespace VerificationPortal.Models
{
    public class VerificationPageContext
    {
        public AffInstitutionsDetail Institution { get; set; } = null!;

        public string CollegeCode => Institution.CollegeCode;

        public string FacultyCode => Institution.FacultyCode;

        public int FacultyCodeInt => int.Parse(Institution.FacultyCode);

        public string InstitutionName => Institution.NameOfInstitution ?? string.Empty;
    }
}
