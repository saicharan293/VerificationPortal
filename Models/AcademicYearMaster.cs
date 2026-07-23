using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class AcademicYearMaster
{
    public int Id { get; set; }

    public string? AcademicYear { get; set; }

    public bool? IsActive { get; set; }

    public int? DisplayOrder { get; set; }
}
