using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class DentalCollegeLandBuildingDetail
{
    public int Id { get; set; }

    public string CollegeCode { get; set; } = null!;

    public int FacultyCode { get; set; }

    public int SeatSlab { get; set; }

    public int SeatIntake { get; set; }

    public bool IsTier2OrHilly { get; set; }

    public decimal? TotalLandAreaAcres { get; set; }

    public string? LandOwnershipType { get; set; }

    public bool? HasFutureExpansionSpace { get; set; }

    public string? SaleDeedDocumentPath { get; set; }

    public string? EncumbranceCertificateDocumentPath { get; set; }

    public string? LandUseCertificateDocumentPath { get; set; }

    public string? ApprovedLayoutPlanDocumentPath { get; set; }

    public string? LandSketchDocumentPath { get; set; }

    public string? DistanceCertificateDocumentPath { get; set; }

    public string? ApprovedBuildingPlanDocumentPath { get; set; }

    public string? CompletionCertificateDocumentPath { get; set; }

    public string? StructuralStabilityCertificateDocumentPath { get; set; }

    public string? FireSafetyNocDocumentPath { get; set; }

    public string? LiftLicenseDocumentPath { get; set; }

    public string? ElectricalSafetyCertificateDocumentPath { get; set; }

    public string? WaterSupplyCertificateDocumentPath { get; set; }

    public string? SewageSanitationApprovalDocumentPath { get; set; }

    public decimal? TotalBuiltupAreaSqm { get; set; }

    public int? LectureHallCount { get; set; }

    public decimal? LectureHallAreaSqm { get; set; }

    public int? LectureHallSeatingCapacity { get; set; }

    public decimal? ExaminationHallAreaSqm { get; set; }

    public decimal? LibraryAreaSqm { get; set; }

    public decimal? HospitalAreaSqm { get; set; }

    public decimal? MuseumDemoRoomsAreaSqm { get; set; }

    public decimal? DepartmentWiseAreaSqm { get; set; }

    public decimal? PreclinicalSkillLabAreaSqm { get; set; }

    public string? LandCategory { get; set; }

    public bool? IsLandInTwoPieces { get; set; }

    public decimal? DistanceBetweenCollegeAndHospitalKm { get; set; }

    public string? Remarks { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? CourseLevel { get; set; }

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

    public virtual AffiliationCollegeMaster CollegeCodeNavigation { get; set; } = null!;

    public virtual Faculty FacultyCodeNavigation { get; set; } = null!;
}
