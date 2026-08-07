using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MedicalUgbedDistribution
{
    public int Id { get; set; }

    public int? GenMedicine { get; set; }

    public int? Paediatrics { get; set; }

    public int? SkinVd { get; set; }

    public int? Psychiatry { get; set; }

    public int? GenSurgery { get; set; }

    public int? Orthopaedics { get; set; }

    public int? Ophthalmology { get; set; }

    public int? Ent { get; set; }

    public int? ObstetricsAnc { get; set; }

    public int? Gynaecology { get; set; }

    public int? Postpartum { get; set; }

    public int? MajorOt { get; set; }

    public int? MinorOt { get; set; }

    public int? Iccu { get; set; }

    public int? Icu { get; set; }

    public int? PicuNicu { get; set; }

    public int? Sicu { get; set; }

    public int? TotalIcubeds { get; set; }

    public int? CasualtyBeds { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? FacultyCode { get; set; }

    public string? CollegeCode { get; set; }

    public string? CourseLevel { get; set; }

    public int? OralMaxillofacialSurgery { get; set; }

    public int? AffiliationTypeId { get; set; }

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

    public virtual TypeOfAffiliation? AffiliationType { get; set; }
}
