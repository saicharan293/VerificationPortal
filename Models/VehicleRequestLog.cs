using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class VehicleRequestLog
{
    public int Id { get; set; }

    public string? CollegeCode { get; set; }

    public string? VehicleRegNo { get; set; }

    public DateTime? RequestTime { get; set; }
}
