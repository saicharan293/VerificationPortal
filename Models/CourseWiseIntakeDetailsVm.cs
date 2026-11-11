using System.ComponentModel.DataAnnotations;

namespace VerificationPortal.Models
{
    public class AcademicIntakePageViewModel1
    {
        public string CollegeCode { get; set; }
        public string FacultyCode { get; set; }
        public string? CollegeName { get; set; }
        public int FacultyId { get; set; }
        public string CourseLevel {get; set;}

        // UG, PG, SS Courses - populated from CollegeCourseIntakeDetails + MstCourses
        public List<IntakeByLevelViewModel1> UgCourses { get; set; } = new List<IntakeByLevelViewModel1>();
        public List<IntakeByLevelViewModel1> PgCourses { get; set; } = new List<IntakeByLevelViewModel1>();
        public List<IntakeByLevelViewModel1> SsCourses { get; set; } = new List<IntakeByLevelViewModel1>();

        // Existing saved data from AcademicIntake table
        public List<SavedAcademicIntakeRowViewModel1> SavedIntakes { get; set; } = new List<SavedAcademicIntakeRowViewModel1>();
        public VerificationPageContextVm PageContext { get; set; } = new();

public TabWiseVerificationResultVm TabwiseVerification { get; set; } = new();

public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();

    }

    // ✅ FULLY CORRECTED - Individual course row for input forms
    public class IntakeByLevelViewModel1
    {
        public int Id { get; set; }

        public string CourseCode { get; set; }
        public string CourseName { get; set; }

        // 2024-25 Data - NULLABLE INTS ✅
        [Display(Name = "AY2024 Existing Intake")]
        public int? AY2024_ExistingIntake { get; set; }

        [Display(Name = "AY2024 Increase Intake")]
        [Range(0, int.MaxValue, ErrorMessage = "Increase intake must be 0 or positive")]
        public int? AY2024_IncreaseIntake { get; set; }

        [Display(Name = "AY2024 Total Intake")]
        [Range(0, int.MaxValue, ErrorMessage = "Total intake must be 0 or positive")]
        public int? AY2024_TotalIntake { get; set; }

        // 2025-26 Data - NULLABLE INTS ✅
        [Display(Name = "AY2025 Existing Intake")]
        public int? AY2025_ExistingIntake { get; set; }

        [Display(Name = "AY2025 LoP/NMC Intake")]
        [Range(0, int.MaxValue, ErrorMessage = "LoP/NMC intake must be 0 or positive")]
        public int? AY2025_LopNmcIntake { get; set; }

        [Display(Name = "AY2025 Total Intake")]
        [Range(0, int.MaxValue, ErrorMessage = "Total intake must be 0 or positive")]
        public int? AY2025_TotalIntake { get; set; }

        // 2025-26 Documents ✅
        [Display(Name = "AY2025 NMC Document")]
        public IFormFile? AY2025_NmcDocument { get; set; }  // File upload

        [Display(Name = "Current NMC Document")]
        public string? AY2025_NmcDocumentUrl { get; set; }  // Existing file URL

        [Display(Name = "AY2025 LoP Date")]
        public DateOnly? AY2025_LopDate { get; set; }  // Matches controller

        [Display(Name = "AY2025 LoP Document")]
        public IFormFile? AY2025_LopDocument { get; set; }

        [Display(Name = "AY2025 LoP Dental Document")]
        public IFormFile? AY2025_LopDentalDocument { get; set; }

        [Display(Name = "Current LoP Document")]
        public string? AY2025_LopDocumentUrl { get; set; }

        [Display(Name = "Current LoP Dental Document")]
        public string? AY2025_LopDentalDocumentUrl { get; set; }

        // 2026-27 Data - NULLABLE INTS ✅
        [Display(Name = "AY2026 Existing Intake")]
        public int? AY2026_ExistingIntake { get; set; }

        [Display(Name = "AY2026 Add Requested Intake")]
        [Range(0, int.MaxValue, ErrorMessage = "Requested intake must be 0 or positive")]
        public int? AY2026_AddRequestedIntake { get; set; }

        [Display(Name = "AY2026 Total Intake")]
        [Range(0, int.MaxValue, ErrorMessage = "Total intake must be 0 or positive")]
        public int? AY2026_TotalIntake { get; set; }
        // 2027-28 Data - NULLABLE INTS ✅
        [Display(Name = "AY2027 Existing Intake")]
        public int? AY2027_ExistingIntake { get; set; }

        [Display(Name = "AY2027 Add Requested Intake")]
        [Range(0, int.MaxValue, ErrorMessage = "Requested intake must be 0 or positive")]
        public int? AY2027_AddRequestedIntake { get; set; }

        [Display(Name = "AY2027 Total Intake")]
        [Range(0, int.MaxValue, ErrorMessage = "Total intake must be 0 or positive")]
        public int? AY2027_TotalIntake { get; set; }
        public IFormFile? AY2025_DCIDocument { get; set; }

        public IFormFile? AY2025_KSDCDocument { get; set; }

        public IFormFile? AY2026_DCIDocument { get; set; }

        public IFormFile? AY2026_KSDCDocument { get; set; }

        public IFormFile? AY2027_DCIDocument { get; set; }

        public IFormFile? AY2027_KSDCDocument { get; set; }

        // =========================
        // VIEW FILE SUPPORT
        // =========================

        public bool HasNmcDocument { get; set; }

        public bool HasLopDocument { get; set; }

        public bool HasAY2025DciDocument { get; set; }

        public bool HasAY2025KsdcDocument { get; set; }

        public bool HasAY2026DciDocument { get; set; }

        public bool HasAY2026KsdcDocument { get; set; }

        public bool HasAY2027DciDocument { get; set; }

        public bool HasAY2027KsdcDocument { get; set; }

        // Optional URLs if needed later

        public string? AY2025_DCIDocumentUrl { get; set; }

        public string? AY2025_KSDCDocumentUrl { get; set; }

        public string? AY2026_DCIDocumentUrl { get; set; }

        public string? AY2026_KSDCDocumentUrl { get; set; }

        public string? AY2027_DCIDocumentUrl { get; set; }

        public string? AY2027_KSDCDocumentUrl { get; set; }
    }

    // Saved data display row (ENHANCED)
    public class SavedAcademicIntakeRowViewModel1
    {
        public int Id { get; set; }
        public string CourseLevel { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;

        [Display(Name = "AY2024 Total")]
        public int AY2024_TotalIntake { get; set; }

        [Display(Name = "AY2025 Total")]
        public int AY2025_TotalIntake { get; set; }

        [Display(Name = "AY2026 Total")]
        public int AY2026_TotalIntake { get; set; }
        public int AY2026_ExistingIntake { get; set; }
        public int AY2026_AddReqIntake { get; set; }

        [Display(Name = "AY2027 Total")]
        public int AY2027_TotalIntake { get; set; }
        public int AY2027_ExistingIntake { get; set; }
        public int AY2027_AddReqIntake { get; set; }


        public string PrincipalName { get; set; }
        public bool HasNmcDocument { get; set; }

        public bool HasKsdcDocument { get; set; }

        public bool Has2025DciDocument { get; set; }
        public bool Has2026DciDocument { get; set; }
        public bool Has2027DciDocument { get; set; }
        public bool HasLopDocument { get; set; }
        public DateTime CreatedOn { get; set; }

        // Helper properties for display
        public string NmcDocumentDisplay => HasNmcDocument ? "✅ Available" : "❌ None";
        public string LopDocumentDisplay => HasLopDocument ? "✅ Available" : "❌ None";
    }
}
