using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class CaMstExaminationScheme
{
    public int SchemeId { get; set; }

    public string SchemeCode { get; set; } = null!;

    public virtual ICollection<CaExaminationScheme> CaExaminationSchemes { get; set; } = new List<CaExaminationScheme>();
}
