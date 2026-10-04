using System.Collections.Generic;

namespace VerificationPortal.Models
{
    public class WorkShopDetailsVerificationVm
    {
        // Institution metadata
        public string CollegeCode { get; set; } = string.Empty;
        public string InstitutionName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string UserDesignation { get; set; } = string.Empty;

        // Faculty context
        public int FacultyCode { get; set; }

        // Workshop information
        public List<WorkShopDetail> WorkshopDetails { get; set; } = new();

        // Section-level verification feedback
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }
}