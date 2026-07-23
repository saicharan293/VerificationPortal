using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class AffHostelFacilityDetail
{
    public int Id { get; set; }

    public int FacilityId { get; set; }

    public string FacultyCode { get; set; } = null!;

    public string CollegeCode { get; set; } = null!;

    public string FacilityName { get; set; } = null!;

    public bool IsAvailable { get; set; }
}
