namespace VerificationPortal.Models
{
    public class TeachingExperienceVerificationViewModel
    {
        public string? CollegeCode { get; set; }

        public int FacultyCode { get; set; }

        public int AffiliationType { get; set; }

        public List<TeachingExperienceFacultyRow> FacultyRows
        {
            get; set;
        } = new();
    }


    public class TeachingExperienceFacultyRow
    {
        public string? NameOfFaculty { get; set; }

        public decimal? TotalExperience { get; set; }

        public List<TeachingExperienceDepartmentRow> Departments
        {
            get; set;
        } = new();
    }


    public class TeachingExperienceDepartmentRow
    {
        public string? DepartmentCode { get; set; }

        public string? DepartmentName { get; set; }

        public string? CourseLevel { get; set; }

        public List<TeachingExperienceDetailRow> Experiences
        {
            get; set;
        } = new();
    }


    public class TeachingExperienceDetailRow
    {
        public int Id { get; set; }

        public string? DesignationCode { get; set; }

        public string? DesignationName { get; set; }

        public string? CourseLevel { get; set; }

        public DateOnly? UgFrom { get; set; }

        public DateOnly? UgTo { get; set; }

        public DateOnly? PgFrom { get; set; }

        public DateOnly? PgTo { get; set; }

        public string? UgCollegeCode { get; set; }

        public string? PgCollegeCode { get; set; }

        public string? UgCollegeName { get; set; }

        public string? PgCollegeName { get; set; }

        public decimal? TotalExperience { get; set; }

        public int? FacultyDetailId { get; set; }
    }
}