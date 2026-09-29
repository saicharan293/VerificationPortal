namespace VerificationPortal.Models
{
    public class AffiliationSelectorVM
    {
        public List<TypeOfAffiliationOptionVM> AffiliationTypes { get; set; } = new();

        public List<CourseLevelOptionVM> CourseLevels { get; set; }  = new();

        public int? SelectedAffiliationTypeId { get; set; }

        public string? SelectedCourseLevel { get; set; }
        public string? Message { get; set; }

        public bool HasCollegeAssignment { get; set; }
        public bool ShowAffiliationSelector { get; set; }
    }


    public class AffiliationContextRequest
    {
        public int AffiliationTypeId { get; set; }

        public string CourseLevel { get; set; } = string.Empty;
    }

    public class TypeOfAffiliationOptionVM
    {
        public int TypeId { get; set; }

        public string TypeDescription { get; set; } = "";
    }

    public class CourseLevelOptionVM
    {
        public string Level { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public string Icon { get; set; } = "";
    }
}
