namespace VerificationPortal.Models
{
    public class DentalLandBuildingVerificationVm
    {
        public DentalCollegeLandBuildingDetail LandBuilding { get; set; } = null!;

        public List<VerificationDocumentVm> LandDocuments { get; set; } = new();

        public List<VerificationDocumentVm> BuildingDocuments { get; set; } = new();

        public SectionFeedbackViewModel? LandFeedback { get; set; }

        public SectionFeedbackViewModel? BuildingFeedback { get; set; }

        public int FacultyId { get; set; }

        public string? CollegeCode { get; set; }
    }

    public class VerificationDocumentVm
    {
        public string Label { get; set; } = string.Empty;

        public string? Path { get; set; }

        public string DocumentType { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public int? DocumentId { get; set; }
    }
}
