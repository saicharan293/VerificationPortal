using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class UserDetail
{
    public int UserDetailsId { get; set; }

    public string CollegeCode { get; set; } = null!;

    public int FacultyId { get; set; }

    public int TypeId { get; set; }

    public string? CourseLevel { get; set; }

    public int? NoOfTeachingStaff { get; set; }

    public int? NoOfResearchScholarsAssistants { get; set; }

    public int? NoOfPostGraduateStudents { get; set; }

    public int? NoOfUnderGraduateStudents { get; set; }

    public int? NoOfAdministrativeStaff { get; set; }

    public int? NoOfParaMedicalStaff { get; set; }

    public int? NoOfOutsiders { get; set; }

    public bool? ProvideUserEducationProgrammes { get; set; }

    public bool IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

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

    public virtual AffiliationCollegeMaster CollegeCodeNavigation { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual TypeOfAffiliation Type { get; set; } = null!;
}
