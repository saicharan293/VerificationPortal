using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MstTab
{
    public int TabId { get; set; }

    public int FacultyId { get; set; }

    public string TabName { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<MstSection> MstSections { get; set; } = new List<MstSection>();

    public virtual ICollection<SectionWiseFeedback> SectionWiseFeedbacks { get; set; } = new List<SectionWiseFeedback>();
}
