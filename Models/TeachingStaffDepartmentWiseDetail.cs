using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class TeachingStaffDepartmentWiseDetail
{
    public int Id { get; set; }

    public string? CollegeCode { get; set; }

    public string? FacultyCode { get; set; }

    public string? CourseLevel { get; set; }

    public string? DepartmentCode { get; set; }

    public string? DesignationCode { get; set; }

    public string? DesignationName { get; set; }

    public DateOnly? Ugfrom { get; set; }

    public DateOnly? Ugto { get; set; }

    public DateOnly? Pgfrom { get; set; }

    public DateOnly? Pgto { get; set; }

    public decimal? TotalExperience { get; set; }

    public string? UgcollegeCode { get; set; }

    public string? PgcollegeCode { get; set; }

    public string? NameOfFaculty { get; set; }

    public int? FacultyDetailId { get; set; }

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
