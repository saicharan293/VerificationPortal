using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class CaMstMediLibraryService
{
    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }
}
