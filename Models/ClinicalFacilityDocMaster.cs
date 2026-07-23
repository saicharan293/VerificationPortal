using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class ClinicalFacilityDocMaster
{
    public int DocId { get; set; }

    public string? DocumentName { get; set; }

    public int FacultyId { get; set; }
}
