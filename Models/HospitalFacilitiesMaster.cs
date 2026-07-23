using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class HospitalFacilitiesMaster
{
    public int FacilityId { get; set; }

    public int AffiliationTypeId { get; set; }

    public string FacultyCode { get; set; } = null!;

    public string FacilityName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }
}
