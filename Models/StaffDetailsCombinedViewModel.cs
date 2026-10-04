using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VerificationPortal.Models
{
    // Combined view model for the page
    public class StaffDetailsCombinedViewModel
    {
        public string? CourseLevel { get; set; }
        public string? CollegeCode { get; set; }
        public string? FacultyCode { get; set; }

        public List<Med_CA_StaffParticularsVM> StaffPayScaleList { get; set; }
            = new();

        public CA_Med_StaffParticularsOtherVM StaffOther { get; set; }
            = new();

        public List<string> ExistingCourseLevels { get; set; }
            = new();

        // Separate verification for each section
        public VerificationFeedbackViewModel? StaffPayScaleVerification { get; set; }

        public VerificationFeedbackViewModel? StaffOtherDetailsVerification { get; set; }
    }

    public class VerificationFeedbackViewModel
    {
        public string? ExistingRemarks { get; set; }

        public string? ExistingStatus { get; set; }

        public string? ExistingStatusClass { get; set; }

        public string? VerifiedBy { get; set; }

        public string? VerifiedDate { get; set; }

        public bool ShowFeedbackForm { get; set; }
    }

    public class Med_CA_StaffParticularsVM
    {
        public int DesignationSlNo { get; set; }
        public string Designation { get; set; } = string.Empty;

        // ✅ Nullable so default textbox shows BLANK
        [Required(ErrorMessage = "Pay Scale is required")]
        public decimal? PayScale { get; set; }
    }

    public class CA_Med_StaffParticularsOtherVM
    {
        public VerificationPageContextVm PageContext { get; set; } = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; }
        public int Id { get; set; }

        public string? CourseLevel { get; set; }
        public string? FacultyCode { get; set; }
        public string? CollegeCode { get; set; }
        public string? RegistrationNo { get; set; }
        public string? SubFacultyCode { get; set; }

        // ✅ Dropdown validations
        [Required(ErrorMessage = "Please select Teachers Updated in EMS")]
        public string? TeachersUpdatedInEMS { get; set; }

        [Required(ErrorMessage = "Please select Examiner Details Attached")]
        public string? ExaminerDetailsAttached { get; set; }

        public IFormFile? TeachersUpdatedPdf { get; set; }

        // Saved File Names (for view button)
        public string? ExaminerDetailsPdfName { get; set; }
        public int? ExaminerDetailsPdfId { get; set; }
        public string? ExaminerDetailsPdfName2 { get; set; }
        public int? ExaminerDetailsPdf2Id { get; set; }
        public string? ExaminerDetailsPdfName3 { get; set; }
        public int? ExaminerDetailsPdf3Id { get; set; }
        public string? ExaminerDetailsPdfName4 { get; set; }
        public int? ExaminerDetailsPdf4Id { get; set; }
        public string? ExaminerDetailsPdfName5 { get; set; }
        public int? ExaminerDetailsPdf5Id { get; set; }
        public string? AEBASLastThreeMonthsPdfName { get; set; }
        public int? AEBASLastThreeMonthsPdfId { get; set; }
        public string? AEBASInspectionDayPdfName { get; set; }
        public int? AEBASInspectionDayPdfId { get; set; }

        [Required(ErrorMessage = "Please select Service Register option")]
        public string? ServiceRegisterMaintained { get; set; }

        [Required(ErrorMessage = "Please select Acquittance Register option")]
        public string? AcquittanceRegisterMaintained { get; set; }

        public string? ProvidentFundPdfName { get; set; }
        public int? ProvidentFundPdfId { get; set; }

        public string? TeachersUpdatedPdfName { get; set; }
        public int? TeachersUpdatedPdfId { get; set; }
        public string? ESIPdfName { get; set; }
        public int? ESIPdfId { get; set; }
    }
}
