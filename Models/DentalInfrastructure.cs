using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class DentalInfrastructure
{
    public int Id { get; set; }

    public int FacultyCode { get; set; }

    public int AffiliationTypeId { get; set; }

    public string CollegeCode { get; set; } = null!;

    public int HospitalDetailsId { get; set; }

    public int RequirementId { get; set; }

    public int SeatSlab { get; set; }

    public decimal RequiredAreaSqFt { get; set; }

    public decimal AvailableAreaSqFt { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? CourseLevel { get; set; }

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

    public virtual TypeOfAffiliation AffiliationType { get; set; } = null!;

    public virtual AffiliationCollegeMaster CollegeCodeNavigation { get; set; } = null!;

    public virtual Faculty FacultyCodeNavigation { get; set; } = null!;

    public virtual HospitalDetailsForAffiliation HospitalDetails { get; set; } = null!;

    public virtual MstDentalInfrastructure Requirement { get; set; } = null!;
}
