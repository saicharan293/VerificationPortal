using System;
using System.Collections.Generic;

namespace VerificationPortal.Models
{
    public class LibraryStaffVerificationVm
    {
        public VerificationPageContextVm PageContext { get; set; } = new();

        public List<LibraryStaffItemVm> Staff { get; set; } = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }

    public class LibraryStaffItemVm
    {
        public int LibraryStaffId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public string? Qualification { get; set; }

        public string? CourseLevel { get; set; }

        public DateOnly? ExperienceFrom { get; set; }

        public DateOnly? ExperienceTo { get; set; }

        public string? PayScale { get; set; }

        public string? Category { get; set; }
    }
}