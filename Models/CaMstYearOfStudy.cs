using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class CaMstYearOfStudy
{
    public int YearOfStudyId { get; set; }

    public string YearName { get; set; } = null!;

    public virtual ICollection<CaAcademicPerformance> CaAcademicPerformances { get; set; } = new List<CaAcademicPerformance>();
}
