using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class CourseIntakeDetail
{
    public int IntakeId { get; set; }

    public string? CourseName { get; set; }

    public byte[]? CourseCode { get; set; }

    public int? IntakeCount { get; set; }
}
