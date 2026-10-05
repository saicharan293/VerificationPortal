using System.Collections.Generic;

namespace VerificationPortal.Models
{
    public class UserDetailsVerificationVm
    {
        public VerificationPageContextVm PageContext { get; set; } = new();

        public UserDetailsItemVm Details { get; set; } = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }

    public class UserDetailsItemVm
    {
        public int UserDetailsId { get; set; }

        public string? CourseLevel { get; set; }

        public int? NoOfTeachingStaff { get; set; }

        public int? NoOfResearchScholarsAssistants { get; set; }

        public int? NoOfPostGraduateStudents { get; set; }

        public int? NoOfUnderGraduateStudents { get; set; }

        public int? NoOfAdministrativeStaff { get; set; }

        public int? NoOfParaMedicalStaff { get; set; }

        public int? NoOfOutsiders { get; set; }

        public bool? ProvideUserEducationProgrammes { get; set; }
    }
}