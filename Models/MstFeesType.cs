using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MstFeesType
{
    public int FeesCode { get; set; }

    public string? FeesType { get; set; }

    public int FacultyId { get; set; }
}
