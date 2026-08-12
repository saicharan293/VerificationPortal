using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class FacultyDetail
{
    public int Id { get; set; }

    public string? FacultyCode { get; set; }

    public string? CollegeCode { get; set; }

    public string NameOfFaculty { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public string? RecognizedPgTeacher { get; set; }

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Pan { get; set; } = null!;

    public string Aadhaar { get; set; } = null!;

    public string? DepartmentDetails { get; set; }

    public string? RemoveRemarks { get; set; }

    public bool? IsRemoved { get; set; }

    public string? RecognizedPhDteacher { get; set; }

    public string? LitigationPending { get; set; }

    public string? IsExaminer { get; set; }

    public string? ExaminerFor { get; set; }

    public string? GuideRecognitionDocPath { get; set; }

    public string? PhDrecognitionDocPath { get; set; }

    public string? LitigationDocPath { get; set; }

    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }

    public bool? IsDeoVerified { get; set; }

    public string? DeoRemarks { get; set; }

    public DateTime? DeoVerifiedDate { get; set; }

    public string? DeoName { get; set; }

    public bool? IsJrVerified { get; set; }

    public string? JrRemarks { get; set; }

    public DateTime? JrVerifiedDate { get; set; }

    public string? JrName { get; set; }

    public bool? IsSoVerified { get; set; }

    public string? SoRemarks { get; set; }

    public DateTime? SoVerifiedDate { get; set; }

    public string? SoName { get; set; }

    public bool? IsArVerified { get; set; }

    public string? ArRemarks { get; set; }

    public DateTime? ArVerifiedDate { get; set; }

    public string? ArName { get; set; }

    public bool? IsRgVerified { get; set; }

    public string? RgRemarks { get; set; }

    public DateTime? RgVerifiedDate { get; set; }

    public string? RgName { get; set; }

    public bool? IsReVerified { get; set; }

    public string? ReRemarks { get; set; }

    public DateTime? ReVerifiedDate { get; set; }

    public string? ReName { get; set; }

    public bool? IsDrVerified { get; set; }

    public string? DrRemarks { get; set; }

    public DateTime? DrVerifiedDate { get; set; }

    public string? DrName { get; set; }

    public bool? IsVcVerified { get; set; }

    public string? VcRemarks { get; set; }

    public DateTime? VcVerifiedDate { get; set; }

    public string? VcName { get; set; }

    public string? CurrentVerificationLevel { get; set; }

    public string? OverallStatus { get; set; }

    public DateTime? LastUpdatedDate { get; set; }
}
