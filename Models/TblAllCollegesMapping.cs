using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class TblAllCollegesMapping
{
    public int Id { get; set; }

    public int? SelectedId { get; set; }

    public string FromCollegeCode { get; set; } = null!;

    public string ToCollegeCode { get; set; } = null!;

    public string FromLetter { get; set; } = null!;

    public string ToLetter { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<TblCollegeMapping> TblCollegeMappings { get; set; } = new List<TblCollegeMapping>();
}
