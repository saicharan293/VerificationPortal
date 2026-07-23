using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class CaMstMedLibTechnicalProcess
{
    public int SlNo { get; set; }

    public string FacultyCode { get; set; } = null!;

    public string ProcessName { get; set; } = null!;
}
