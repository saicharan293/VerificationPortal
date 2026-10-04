namespace VerificationPortal.Models
{
    public class DentalFieldPracticeAreaVerificationVm
    {
        public VerificationPageContextVm PageContext { get; set; } = new();

        public RuralFieldPracticeAreaVm RuralFieldPracticeArea { get; set; } = new();
        public DentalFieldPracticeAreaVm UrbanFieldPracticeArea { get; set; } = new();

        public List<MstFieldTypeChp> FieldTypes { get; set; } = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }

    public class FieldPraciceAreaVm
    {
        public DentalFieldPracticeArea FieldPracticeArea { get; set; } = new();
        public int? StaffListId { get; set; }
    }

    public class RuralFieldPracticeAreaVm: FieldPraciceAreaVm { }
    public class DentalFieldPracticeAreaVm: FieldPraciceAreaVm { }
}
