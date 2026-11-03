using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public class NonTeachingStaffVerificationVm
{
    public VerificationPageContextVm PageContext { get; set; } = new();

    public List<NonTeachingStaffItemVm> Staff { get; set; } = new();

    public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
}

public class NonTeachingStaffItemVm
{
    public int Id { get; set; }

    public string? CollegeCode { get; set; }

    public string? FacultyCode { get; set; }

    public string? CourseLevel { get; set; }

    public string? StaffName { get; set; }

    public string? Designation { get; set; }

    public string? MobileNumber { get; set; }

    public string? SalaryPaid { get; set; }
}