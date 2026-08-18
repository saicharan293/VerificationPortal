using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class SectionWiseFeedback
{
    public int SectionWiseFeedbackId { get; set; }

    public int FacultyId { get; set; }

    public string CollegeCode { get; set; } = null!;

    public int TabId { get; set; }

    public int SectionId { get; set; }

    public string? VerifiedBy { get; set; }

    public DateTime? VerifiedOn { get; set; }

    public string? VerificationStatus { get; set; }

    public string? Remarks { get; set; }

    public virtual AffiliationCollegeMaster CollegeCodeNavigation { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual MstSection Section { get; set; } = null!;

    public virtual MstTab Tab { get; set; } = null!;
}
