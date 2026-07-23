using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MstHospitalLocation
{
    public int FacultyId { get; set; }

    public int LocationId { get; set; }

    public string? LocationDescription { get; set; }
}
