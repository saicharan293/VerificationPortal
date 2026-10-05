using VerificationPortal.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VerificationPortal.Models
{
    public class LibraryExpenditureVerificationVm
    {
        // Common verification page context
        public VerificationPageContextVm PageContext { get; set; } = new();

        // College library expenditure records
        public List<LibraryExpenditureItemVm> Items { get; set; } = new();

        public List<LibraryServiceItemVm> Services { get; set; } = new();

        // Feedback for this verification page
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }

    public class LibraryExpenditureItemVm
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal? ExpenditureProposed {  get; set; }

        public string? CourseLevel { get; set; }
    }

    public class LibraryServiceItemVm
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string? CourseLevel { get; set; }
    }
}