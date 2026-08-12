using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class HospitalDetailsForAffiliation
{
    public int HospitalDetailsId { get; set; }

    public int AffiliationTypeId { get; set; }

    public string CollegeCode { get; set; } = null!;

    public string FacultyCode { get; set; } = null!;

    public bool? ParentMedicalCollegeExists { get; set; }

    public string? HospitalType { get; set; }

    public string? HospitalOwnedBy { get; set; }

    public string? HospitalOwnerName { get; set; }

    public string HospitalDistrictId { get; set; } = null!;

    public string HospitalTalukId { get; set; } = null!;

    public string? Location { get; set; }

    public int? TotalBeds { get; set; }

    public int? OpdperDay { get; set; }

    public decimal? IpdbedOccupancyPercent { get; set; }

    public int? AnnualOpdprevYear { get; set; }

    public int? AnnualIpdprevYear { get; set; }

    public bool? IsParentHospitalForOtherNursingInstitution { get; set; }

    public decimal? DistanceBetweenCollegeAndHospitalKm { get; set; }

    public bool? IsOwnerAmemberOfTrust { get; set; }

    public string? HospitalName { get; set; }

    public byte[]? HospitalParentSupportingDoc { get; set; }

    public string? CourseLevel { get; set; }

    public int? DentalChairsCount { get; set; }

    public bool? Has24HourEmergency { get; set; }

    public bool? HasCriticalCareServices { get; set; }

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

    public virtual ICollection<DentalInfrastructure> DentalInfrastructures { get; set; } = new List<DentalInfrastructure>();

    public virtual ICollection<DentalService> DentalServices { get; set; } = new List<DentalService>();

    public virtual ICollection<DentalWardBedDistribution> DentalWardBedDistributions { get; set; } = new List<DentalWardBedDistribution>();

    public virtual ICollection<HospitalDocumentsToBeUploaded> HospitalDocumentsToBeUploadeds { get; set; } = new List<HospitalDocumentsToBeUploaded>();

    public virtual ICollection<HospitalFacility> HospitalFacilities { get; set; } = new List<HospitalFacility>();

    public virtual ICollection<IndoorInfrastructureRequirementsCompliance> IndoorInfrastructureRequirementsCompliances { get; set; } = new List<IndoorInfrastructureRequirementsCompliance>();

    public virtual ICollection<MedicalAlliedDisciplineDetail> MedicalAlliedDisciplineDetails { get; set; } = new List<MedicalAlliedDisciplineDetail>();

    public virtual ICollection<SuperVisionInFieldPracticeArea> SuperVisionInFieldPracticeAreas { get; set; } = new List<SuperVisionInFieldPracticeArea>();
}
