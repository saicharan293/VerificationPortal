using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class CaMedResearchPublicationsDetail
{
    public int SlNo { get; set; }

    public string CollegeCode { get; set; } = null!;

    public string FacultyCode { get; set; } = null!;

    public string? SubFacultyCode { get; set; }

    public string? RegistrationNo { get; set; }

    public int? PublicationsNo { get; set; }

    public string? PublicationsPdfName { get; set; }

    public string? Pi { get; set; }

    public int? Rguhsfunded { get; set; }

    public int? ExternalBodyFunding { get; set; }

    public string? ProjectsPdfName { get; set; }

    public string? ClinicalTrialsPdfName { get; set; }

    public int? StudentsRguhsfunded { get; set; }

    public int? StudentsExternalBodyFunding { get; set; }

    public string? StudentsProjectsPdfName { get; set; }

    public int? FacultyRguhsfunded { get; set; }

    public int? FacultyExternalBodyFunding { get; set; }

    public string? FacultyProjectsPdfName { get; set; }

    public string? CourseLevel { get; set; }

    public string? PublicationsPdfPath { get; set; }

    public string? ProjectsPdfPath { get; set; }

    public string? ClinicalTrialsPdfPath { get; set; }

    public string? StudentsProjectsPdfPath { get; set; }

    public string? FacultyProjectsPdfPath { get; set; }

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
