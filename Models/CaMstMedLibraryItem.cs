using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class CaMstMedLibraryItem
{
    public int SlNo { get; set; }

    public string FacultyCode { get; set; } = null!;

    public string ItemName { get; set; } = null!;
}
