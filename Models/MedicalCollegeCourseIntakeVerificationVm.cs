using VerificationPortal.Services.Verification.Models;

namespace VerificationPortal.Models
{
    public class MedicalCollegeCourseIntakeVerificationVm
    {
        public string CollegeCode { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;

        public int FacultyCode { get; set; }
        public string CourseLevel { get; set; } = string.Empty;

        public List<MedicalCollegeCourseIntakeVerificationRowVm> Courses { get; set; } = new();

        public List<PreviousIntakeCourseVerificationVm> PreviousDetails { get; set; } = new();

        public bool ShowIncreasedIntakeFields { get; set; }

        // Page Context
        public VerificationPageContextVm PageContext { get; set; } = new();

        // Tab-wise Verification
        public TabWiseVerificationResultVm TabwiseVerification { get; set; } = new();

        // Section Feedback
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();

        // Verification Details
        public VerificationDisplayModel? Verification { get; set; }
    }

    public class MedicalCollegeCourseIntakeVerificationRowVm
    {
        public int Slno { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public int Intake2627 { get; set; }

        public string? IncreasedIntake { get; set; }

        public string? AcademicYear { get; set; }
    }
    
    public class PreviousIntakeSlabVerificationVm
    {
        public int Id { get; set; }

        public string IntakeSlab { get; set; } = string.Empty;

        public short? LopYear { get; set; }

        public bool HasDocument { get; set; }

        public string? DocumentName { get; set; }

        // Document URL or download endpoint
        public string? DocumentUrl { get; set; }
    }

    public class PreviousIntakeCourseVerificationVm
    {
        public int CourseCode { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public List<PreviousIntakeSlabVerificationVm> Slabs { get; set; } = new();
    }
}
