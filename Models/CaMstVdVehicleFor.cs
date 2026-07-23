using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class CaMstVdVehicleFor
{
    public int Id { get; set; }

    public int FacultyCode { get; set; }

    public string VehicleForCode { get; set; } = null!;

    public string VehicleForName { get; set; } = null!;
}
