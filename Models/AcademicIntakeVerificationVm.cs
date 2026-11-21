
using System;
using System.Collections.Generic;

namespace VerificationPortal.Models.Verification
{
    public class AcademicIntakeVerificationVm
    {
        // Academic Intake
        public string CollegeCode { get; set; } = string.Empty;
        public string FacultyCode { get; set; } = string.Empty;
        public string? CollegeName { get; set; }
        public int FacultyId { get; set; }

        public List<IntakeVerificationCourseVm> UgCourses { get; set; } = new();
        public List<IntakeVerificationCourseVm> PgCourses { get; set; } = new();
        public List<IntakeVerificationCourseVm> SsCourses { get; set; } = new();

        public List<SavedAcademicIntakeVerificationVm> SavedIntakes { get; set; } = new();

        // Institution details
        public int InstitutionId { get; set; }
        public string TypeOfInstitution { get; set; } = string.Empty;
        public string NameOfInstitution { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? VillageTownCity { get; set; }
        public string? Taluk { get; set; }
        public string? District { get; set; }
        public string? PinCode { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailId { get; set; }
        public string? CourseLevel { get; set; }

        // Verification context
        public VerificationPageContextVm PageContext { get; set; } = new();

        public TabWiseVerificationResultVm? TabwiseVerification { get; set; }

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; }
    }


    public class IntakeVerificationCourseVm
    {
        public int Id { get; set; }

        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CourseLevel { get; set; } = string.Empty;

        // AY 2024-25
        public int? AY2024_ExistingIntake { get; set; }
        public int? AY2024_IncreaseIntake { get; set; }
        public int? AY2024_TotalIntake { get; set; }

        // AY 2025-26
        public int? AY2025_ExistingIntake { get; set; }
        public int? AY2025_LopNmcIntake { get; set; }
        public int? AY2025_TotalIntake { get; set; }
        public DateOnly? AY2025_LopDate { get; set; }

        // AY 2026-27
        public int? AY2026_ExistingIntake { get; set; }
        public int? AY2026_AddRequestedIntake { get; set; }
        public int? AY2026_TotalIntake { get; set; }
        public DateOnly? AY2026_LopDate { get; set; }              // new

        // AY 2027-28
        public int? AY2027_ExistingIntake { get; set; }
        public int? AY2027_TotalIntake { get; set; }

        // Document availability
        public bool HasNmcDocument { get; set; }
        public bool HasLopDocument { get; set; }

        public bool HasAY2025DciDocument { get; set; }
        public bool HasAY2025KsdcDocument { get; set; }

        public bool HasAY2026LopDocument { get; set; }             // new
        public bool HasAY2026DciDocument { get; set; }

        public bool HasAY2027DciDocument { get; set; }

        // Document paths for viewing/downloading
        public string? AY2025_NmcDocumentUrl { get; set; }
        public string? AY2025_LopDocumentUrl { get; set; }
        public string? AY2025_LopDentalDocumentUrl { get; set; }

        public string? AY2025_DCIDocumentUrl { get; set; }
        public string? AY2025_KSDCDocumentUrl { get; set; }

        public string? AY2026_LopDocumentUrl { get; set; }         // new
        public string? AY2026_DCIDocumentUrl { get; set; }

        public string? AY2027_DCIDocumentUrl { get; set; }
    }

    public class SavedAcademicIntakeVerificationVm
    {
        public int Id { get; set; }

        public string CourseLevel { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;

        public int AY2024_TotalIntake { get; set; }
        public int AY2025_TotalIntake { get; set; }
        public int AY2026_TotalIntake { get; set; }
        public int AY2027_TotalIntake { get; set; }

        public string? PrincipalName { get; set; }

        public bool HasNmcDocument { get; set; }
        public bool HasLopDocument { get; set; }
        public bool HasDciDocument { get; set; }
        public bool HasKsdcDocument { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
