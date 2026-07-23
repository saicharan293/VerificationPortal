using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class TblCollegeMapping
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public int FacultyCode { get; set; }

    public string CollegeFrom { get; set; } = null!;

    public string CollegeTo { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual Faculty FacultyCodeNavigation { get; set; } = null!;
}
