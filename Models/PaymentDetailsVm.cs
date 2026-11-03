using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

/// <summary>
/// Unified view model for the Payment Details page.
/// Renders differently depending on faculty code:
///   Medical (1) -> PaymentAffiliationDocument
///   Dental  (2) -> TxnDentalPayment
/// </summary>
public class PaymentDetailsVm
{
    public VerificationPageContextVm PageContext { get; set; } = new();

    // Which faculty is being verified: "Medical" or "Dental"
    public string FacultyType { get; set; } = "Medical";

    // Whether data was found for the selected faculty
    public bool HasPayment { get; set; }

    // ---- Common fields (mapped from either entity) ----
    public string? CollegeCode { get; set; }
    public string? CourseLevel { get; set; }
    public string? TransactionId { get; set; }
    public decimal? PaymentAmount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? ReceiptPath { get; set; }

    // Medical-specific
    public string? ScreenshotFileName { get; set; }
    public string? ScreenshotFilePath { get; set; }
    public Guid? PublicAccessToken { get; set; }

    // Affiliation type details (from MstAffiliationType - Medical only)
    public int AffiliationTypeId { get; set; }
    public string? AffiliationCategory { get; set; }
    public string? FormNo { get; set; }
    public string? AcademicYear { get; set; }
    public string? CourseLevelGroup { get; set; }

    // TypeOfAffiliation description (from TypeOfAffiliation table)
    public string? TypeOfAffiliationDescription { get; set; }

    // Section feedback
    public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
}