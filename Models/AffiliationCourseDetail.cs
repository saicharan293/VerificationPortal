using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class AffiliationCourseDetail
{
    public int Id { get; set; }

    public string Facultycode { get; set; } = null!;

    public string Collegecode { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public string? IntakeDuring202526 { get; set; }

    public string? IntakeSlab { get; set; }

    public string? Typeofpermission { get; set; }

    public DateOnly? YearofLop { get; set; }

    public string? DateOfLoprenewalGoimci { get; set; }

    public string? SanctionedIntakePermission { get; set; }

    public string? Dateofrecognition { get; set; }

    public DateOnly? YearofObtainingEcandFc { get; set; }

    public string? SannctionedIntakeEcFc { get; set; }

    public string? YearOfLastAffiliationRguhs { get; set; }

    public string? SanctionedIntakeLastAffiliation { get; set; }

    public DateOnly? DateOfPreviousLicinspection { get; set; }

    public string? ActionTakenOnDeficiencies { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? GokorderPath { get; set; }

    public string? PreviousNotificationFilesPath { get; set; }

    public string? LastAffiliationRguhsfilePath { get; set; }

    public string? DateOfLoprenewalDciksdc { get; set; }

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
