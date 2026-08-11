using System;
using System.Collections.Generic;

namespace VerificationPortal.Models;

public partial class AffHostelDetail
{
    public int HostelDetailsId { get; set; }

    public string FacultyCode { get; set; } = null!;

    public string CollegeCode { get; set; } = null!;

    public string HostelType { get; set; } = null!;

    public string BuiltUpAreaSqFt { get; set; } = null!;

    public bool HasSeparateHostel { get; set; }

    public bool SeparateProvisionMaleFemale { get; set; }

    public string TotalFemaleStudents { get; set; } = null!;

    public string TotalFemaleRooms { get; set; } = null!;

    public string TotalMaleStudents { get; set; } = null!;

    public string TotalMaleRooms { get; set; } = null!;

    public string? PossessionProofPath { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public bool? CommonRoomMen { get; set; }

    public bool? CommonRoomWomen { get; set; }

    public string? AnyOtherFacility { get; set; }

    public string? HostelFacilityDetails { get; set; }

    public int? HostelMenCount { get; set; }

    public int? HostelWomenCount { get; set; }

    public string? OwnOrRented { get; set; }

    public decimal? SpacePerStudent { get; set; }

    public bool? SleepingFurniture { get; set; }

    public bool? SanitaryBathing { get; set; }

    public bool? DiningHall { get; set; }

    public bool? HostelCommonRoom { get; set; }

    public bool? VisitorsRoom { get; set; }

    public bool? KitchenPantry { get; set; }

    public bool? WardenOffice { get; set; }

    public bool? ReceptionCounter { get; set; }

    public bool? GamesRecreation { get; set; }

    public bool? MedicalFacilities { get; set; }

    public string? CourseLevel { get; set; }

    public string? MenHostelAreaSqFt { get; set; }

    public string? WomenHostelAreaSqFt { get; set; }

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
