using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MstSection
{
    public int SectionId { get; set; }

    public int FacultyId { get; set; }

    public int TabId { get; set; }

    public string SectionName { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<SectionWiseFeedback> SectionWiseFeedbacks { get; set; } = new List<SectionWiseFeedback>();

    public virtual MstTab Tab { get; set; } = null!;
}
