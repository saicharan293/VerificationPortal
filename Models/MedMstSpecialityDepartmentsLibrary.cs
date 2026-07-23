using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MedMstSpecialityDepartmentsLibrary
{
    public int Id { get; set; }

    public string FacultyCode { get; set; } = null!;

    public string SpecialityDepartments { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;
}
