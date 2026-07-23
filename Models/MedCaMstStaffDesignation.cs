using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MedCaMstStaffDesignation
{
    public int SlNo { get; set; }

    public string FacultyCode { get; set; } = null!;

    public string? SubFacultyCode { get; set; }

    public string Designation { get; set; } = null!;
}
