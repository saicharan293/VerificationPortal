using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class DesignationMaster
{
    public int Id { get; set; }

    public int FacultyCode { get; set; }

    public string DesignationCode { get; set; } = null!;

    public string DesignationName { get; set; } = null!;

    public int? DesignationOrder { get; set; }
}
