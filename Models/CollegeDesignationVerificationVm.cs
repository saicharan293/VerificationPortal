using System.Collections.Generic;

namespace VerificationPortal.Models
{
    public class CollegeDesignationVerificationVm
    {
        public VerificationPageContextVm PageContext { get; set; } = new();

        public List<CollegeDesignationItemVm> Designations { get; set; } = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }

    public class CollegeDesignationItemVm
    {
        public int Id { get; set; }

        public string FacultyCode { get; set; } = string.Empty;

        public string CollegeCode { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public string? DesignationCode { get; set; }

        public string? Department { get; set; }

        public string? DepartmentCode { get; set; }

        public string? SeatSlabId { get; set; }

        public string RequiredIntake { get; set; } = string.Empty;

        public string AvailableIntake { get; set; } = string.Empty;

        public string? UgRguhsintake { get; set; }

        public string? UgPresentintake { get; set; }

        public string? PgRguhsintake { get; set; }

        public string? Goksanctioned { get; set; }

        public string? Pggoksanctioned { get; set; }

        public string? PgPresentintake { get; set; }
    }
}