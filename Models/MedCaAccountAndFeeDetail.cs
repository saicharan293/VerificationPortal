using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MedCaAccountAndFeeDetail
{
    public int Id { get; set; }

    public string CollegeCode { get; set; } = null!;

    public string FacultyCode { get; set; } = null!;

    public string? SubFacultyCode { get; set; }

    public string? RegistrationNo { get; set; }

    public string? GoverningCouncilPdfName { get; set; }

    public string AuthorityNameAddress { get; set; } = null!;

    public string AuthorityContact { get; set; } = null!;

    public decimal RecurrentAnnual { get; set; }

    public decimal NonRecurrentAnnual { get; set; }

    public decimal Deposits { get; set; }

    public decimal TuitionFee { get; set; }

    public decimal SportsFee { get; set; }

    public decimal UnionFee { get; set; }

    public decimal LibraryFee { get; set; }

    public decimal OtherFee { get; set; }

    public decimal TotalFee { get; set; }

    public string AccountBooksMaintained { get; set; } = null!;

    public string? AccountSummaryPdfName { get; set; }

    public string AccountsAudited { get; set; } = null!;

    public string? AuditedStatementPdfName { get; set; }

    public string CourseLevel { get; set; } = null!;

    public string? DonationLevied { get; set; }

    public string? DonationPdfName { get; set; }

    public string? GoverningCouncilPdfPath { get; set; }

    public string? AccountSummaryPdfPath { get; set; }

    public string? AuditedStatementPdfPath { get; set; }

    public string? DonationPdfPath { get; set; }

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
