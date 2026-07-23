using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MstLibraryEquipmentMaster
{
    public int EquipmentId { get; set; }

    public string? EquipmentName { get; set; }

    public int FacultyId { get; set; }
}
