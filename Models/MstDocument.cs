using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MstDocument
{
    public int DocumentId { get; set; }

    public int FacultyId { get; set; }

    public int TabId { get; set; }

    public int? SectionId { get; set; }

    public string DocumentName { get; set; } = null!;

    public bool IsMandatory { get; set; }

    public int? DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<DocumentWiseFeedback> DocumentWiseFeedbacks { get; set; } = new List<DocumentWiseFeedback>();

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual MstSection? Section { get; set; }

    public virtual MstTab Tab { get; set; } = null!;
}
