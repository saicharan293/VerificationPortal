namespace VerificationPortal.Models
{
    public class VerificationPageContext
    {
        public AffInstitutionsDetail Institution { get; set; } = null!;

        public string CollegeCode => Institution.CollegeCode;

        public string FacultyCode => Institution.FacultyCode;
        public TblRguhsFacultyUser User { get; set; }

        public int FacultyCodeInt => int.Parse(Institution.FacultyCode);

        public string InstitutionName => Institution.NameOfInstitution ?? string.Empty;

        public Faculty Faculty { get; set; } = null!;
        public string FacultyName => Faculty.FacultyName;
    }
}
