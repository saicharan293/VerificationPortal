using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class DocumentWiseFeedback
{
    public int DocumentWiseFeedbackId { get; set; }

    public int FacultyId { get; set; }

    public string CollegeCode { get; set; } = null!;

    public int DocumentId { get; set; }

    public int UserId { get; set; }

    public string? Feedback { get; set; }

    public string Status { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual AffiliationCollegeMaster CollegeCodeNavigation { get; set; } = null!;

    public virtual MstDocument Document { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual TblRguhsFacultyUser User { get; set; } = null!;
}
