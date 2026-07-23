using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class UniversityImage
{
    public int Id { get; set; }

    public string? FileName { get; set; }

    public byte[]? Photo { get; set; }
}
