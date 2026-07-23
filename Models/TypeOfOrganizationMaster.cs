using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class TypeOfOrganizationMaster
{
    public int TypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }
}
