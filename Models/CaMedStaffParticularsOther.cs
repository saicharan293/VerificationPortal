using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class CaMedStaffParticularsOther
{
    public int Id { get; set; }

    public string FacultyCode { get; set; } = null!;

    public string CollegeCode { get; set; } = null!;

    public string? RegistrationNo { get; set; }

    public string? SubFacultyCode { get; set; }

    public string? TeachersUpdatedInEms { get; set; }

    public string? ExaminerDetailsAttached { get; set; }

    public string? ExaminerDetailsPdfName { get; set; }

    public string? AebaslastThreeMonthsPdfName { get; set; }

    public string? AebasinspectionDayPdfName { get; set; }

    public string? ServiceRegisterMaintained { get; set; }

    public string? AcquittanceRegisterMaintained { get; set; }

    public string? ProvidentFundPdfName { get; set; }

    public string? EsipdfName { get; set; }

    public string? CourseLevel { get; set; }

    public string? TeachersUpdatedPdfName { get; set; }

    public string? TeachersUpdatedPdfPath { get; set; }

    public string? ExaminerDetailsPdfPath { get; set; }

    public string? AebaslastThreeMonthsPdfPath { get; set; }

    public string? AebasinspectionDayPdfPath { get; set; }

    public string? ProvidentFundPdfPath { get; set; }

    public string? EsipdfPath { get; set; }

    public string? ExaminerDetailsPdfName2 { get; set; }

    public string? ExaminerDetailsPdfName3 { get; set; }

    public string? ExaminerDetailsPdfName4 { get; set; }

    public string? ExaminerDetailsPdfName5 { get; set; }

    public string? ExaminerDetailsPdfPath2 { get; set; }

    public string? ExaminerDetailsPdfPath3 { get; set; }

    public string? ExaminerDetailsPdfPath4 { get; set; }

    public string? ExaminerDetailsPdfPath5 { get; set; }

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
