using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class TblVerificationLevel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int VerificationLevel { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }
}
