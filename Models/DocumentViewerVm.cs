namespace VerificationPortal.Models;

public class DocumentViewerVm
{
    public string Url { get; set; } = string.Empty;

    public string Title { get; set; } = "Document Viewer";

    public int? DocumentId { get; set; }

    public int? FacultyId { get; set; }
    public string? CollegeCode { get; set; }
}

public class DocumentFeedbackViewModel
{
    // ---------------------------------------------------------
    // DOCUMENT CONTEXT
    // ---------------------------------------------------------

    public int DocumentId { get; set; }

    public int FacultyId { get; set; }

    public string CollegeCode { get; set; } = string.Empty;


    // ---------------------------------------------------------
    // FEEDBACK DETAILS
    // ---------------------------------------------------------

    public string? Feedback { get; set; }

    public string Status { get; set; } = string.Empty;
}