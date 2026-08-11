using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class MedicalDepartmentOfficesMeu
{
    public int Id { get; set; }

    public bool HasHodRoomWithOfficeAndRecords { get; set; }

    public bool HasRoomsForFacultyAndResidents { get; set; }

    public bool FacultyRoomsHaveCommunicationComputerInternet { get; set; }

    public bool HasRoomsForNonTeachingStaff { get; set; }

    public bool HasMedicalEducationUnit { get; set; }

    public decimal? MedicalEducationUnitAreaSqm { get; set; }

    public bool? MedicalEducationUnitHasAudioVisual { get; set; }

    public bool? MedicalEducationUnitHasInternet { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? MeuCoordinatorName { get; set; }

    public string? MeuCoordinatorDesignationDepartment { get; set; }

    public string? MeuCoordinatorPhone { get; set; }

    public string? MeuCoordinatorEmail { get; set; }

    public string? MeuMembersListDescription { get; set; }

    public string? MeuActivitiesLastAcademicYear { get; set; }

    public string? MeuMembersListFilePath { get; set; }

    public string? CollegeCode { get; set; }

    public string? FacultyCode { get; set; }

    public string? CourseLevel { get; set; }

    public bool? HasDentalEducationUnit { get; set; }

    public decimal? DentalEducationUnitAreaSqm { get; set; }

    public bool? DentalEducationUnitHasAudioVisual { get; set; }

    public bool? DentalEducationUnitHasInternet { get; set; }

    public string? DeuCoordinatorName { get; set; }

    public string? DeuCoordinatorDesignationDepartment { get; set; }

    public string? DeuCoordinatorPhone { get; set; }

    public string? DeuCoordinatorEmail { get; set; }

    public string? DeuMembersListDescription { get; set; }

    public string? DeuActivitiesLastAcademicYear { get; set; }

    public string? DeuMembersListFilePath { get; set; }

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
