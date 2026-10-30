
using System.Collections.Generic;

namespace VerificationPortal.Models
{
    public class MedicalLandBuildingVerificationVm
    {
        // Common verification page information
        public VerificationPageContextVm PageContext { get; set; } = new();

        // ==========================================
        // 1. LAND DETAILS - SmallGroupTeaching
        // ==========================================

        public int Id { get; set; }

        public bool? IsMinimumLandAvailable { get; set; }

        public string? LandDetailsIfYes { get; set; }

        public bool? HasPurchasePlanIfNo { get; set; }

        public bool? HasBudgetProvisionIfNo { get; set; }

        public bool? HasFutureExpansionSpace { get; set; }

        public string? LandRecordsFilePath { get; set; }

        // ==========================================
        // 2. BUILDING DETAILS - SmallGroupTeaching
        // ==========================================

        public bool? IsBuildingAsPerCouncilNorms { get; set; }

        public string? LandOwnershipType { get; set; }

        public string? BuildingOwnershipType { get; set; }

        public decimal? FloorAreaSqFt { get; set; }

        public int? NumberOfBlocks { get; set; }

        public int? NumberOfFloors { get; set; }

        public int? YearOfConstruction { get; set; }

        public string? ApprovedBuildingPlanFilePath { get; set; }

        // ==========================================
        // 3. GENERAL SMALL GROUP TEACHING
        // SmallGroupTeaching
        // ==========================================

        public int AnnualMbbsIntake { get; set; }

        public int SmallGroupBatchSize { get; set; }

        public bool TeachingAreasSharedAllDepts { get; set; }

        public bool AvInAllTeachingAreas { get; set; }

        public bool InternetInAllTeachingAreas { get; set; }

        public bool DigitalLinkAllTeachingAreas { get; set; }

        // ==========================================
        // 4. TEACHING ROOM DETAILS
        // SmallGroupTeaching
        // ==========================================

        public int SmallGroupStudents { get; set; }

        public decimal RequiredAreaSqm { get; set; }

        public decimal AvailableAreaSqm { get; set; }

        public decimal AreaDeficiencySqm { get; set; }

        public bool RoomsSharedByAllDepts { get; set; }

        public bool AppropriateAreaEachSpecialty { get; set; }

        public bool ConnectedToLectureHalls { get; set; }

        public bool InternetInTeachingRooms { get; set; }

        // ==========================================
        // 5. ADMINISTRATIVE PHYSICAL FACILITIES
        // MedicalAdministrativePhysicalFacility
        // ==========================================

        public decimal? PrincipalChamberAreaSqFt { get; set; }

        public decimal? OfficeRoomAreaSqFt { get; set; }

        public decimal? StaffRoomsAreaSqFt { get; set; }

        public decimal? LectureHallsAreaSqFt { get; set; }

        public decimal? LaboratoriesAreaSqFt { get; set; }

        public decimal? SeminarHallAreaSqFt { get; set; }

        public decimal? AuditoriumAreaSqFt { get; set; }

        public decimal? MuseumAreaSqFt { get; set; }

        public bool? ExaminationHallAvailable { get; set; }

        public int? WorkshopStaffCount { get; set; }

        public string? WorkshopEquipmentDetails { get; set; }

        public string? WorkshopScopeOfWork { get; set; }

        public decimal? AnimalHouseAreaSqFt { get; set; }

        public int? AnimalHouseStaffCount { get; set; }

        public string? AnimalTypes { get; set; }

        public decimal? CommitteeRoomsAreaSqFt { get; set; }

        public bool? CommonRoomMenAvailable { get; set; }

        public bool? CommonRoomWomenAvailable { get; set; }

        public bool? StudentHostelAvailable { get; set; }

        public bool? StaffQuartersPrincipal { get; set; }

        public bool? StaffQuartersOtherStaff { get; set; }

        public bool? StaffQuartersTeachingAncillary { get; set; }

        public bool? RegisteredUnderAnatomyAct { get; set; }

        public bool? AnimalHouseAvailable { get; set; }

        // ==========================================
        // 6. STUDENT PRACTICAL LABORATORIES
        // MedicalStudentPracticalLab
        // ==========================================

        public bool HistologyAvailable { get; set; }

        public bool HistologyShared { get; set; }

        public bool ClinicalPhysiologyAvailable { get; set; }

        public bool ClinicalPhysiologyShared { get; set; }

        public bool BiochemistryAvailable { get; set; }

        public bool BiochemistryShared { get; set; }

        public bool HistopathCytopathAvailable { get; set; }

        public bool HistopathCytopathShared { get; set; }

        public bool ClinPathHemeAvailable { get; set; }

        public bool ClinPathHemeShared { get; set; }

        public bool MicrobiologyAvailable { get; set; }

        public bool MicrobiologyShared { get; set; }

        public bool ClinicalPharmAvailable { get; set; }

        public bool ClinicalPharmShared { get; set; }

        public bool CalPharmAvailable { get; set; }

        public bool CalPharmShared { get; set; }

        public bool AllLabsHaveAv { get; set; }

        public bool AllLabsHaveInternet { get; set; }

        public bool TechnicalStaffFacilitiesEnsured { get; set; }

        // ==========================================
        // 7. MUSEUM DETAILS
        // MedicalMuseum
        // ==========================================

        public bool SeparateAnatomyMuseumAvailable { get; set; }

        public bool PathologyForensicSharedMuseum { get; set; }

        public bool PharmMicroCommSharedMuseum { get; set; }

        public int SeatingCapacityPerMuseum { get; set; }

        public decimal SeatingAreaAvailableSqm { get; set; }

        public decimal SeatingAreaRequiredSqm { get; set; }

        public decimal SeatingAreaDeficiencySqm { get; set; }

        public bool MuseumsHaveAv { get; set; }

        public bool MuseumsHaveInternet { get; set; }

        public bool MuseumsDigitallyLinked { get; set; }

        public bool MuseumsHaveRacksShelves { get; set; }

        public bool MuseumsHaveRadiologyDisplay { get; set; }

        public bool TeachingTimeSharingProgrammed { get; set; }

        // ==========================================
        // 8. COURSE AND COLLEGE IDENTIFICATION
        // ==========================================

        public string? FacultyCode { get; set; }

        public string? CourseLevel { get; set; }

        // ==========================================
        // 9. DOCUMENT VERIFICATION
        // ==========================================

        public List<VerificationDocumentVm> LandDocuments { get; set; } = new();

        public List<VerificationDocumentVm> BuildingDocuments { get; set; } = new();

        // ==========================================
        // 10. SECTION FEEDBACK
        // ==========================================

        public SectionFeedbackViewModel? LandFeedback { get; set; }

        public SectionFeedbackViewModel? BuildingFeedback { get; set; }
        public SectionFeedbackViewModel? AreaFeedback { get; set; }
        public SectionFeedbackViewModel? FacilitiesFeedback { get; set; }
        public SectionFeedbackViewModel? StaffQuartersFeedback { get; set; }
        public SectionFeedbackViewModel? WorkshopFeedback { get; set; }
        public SectionFeedbackViewModel? AnimalHouseFeedback { get; set; }
        public SectionFeedbackViewModel? SmallGroupFeedback { get; set; }
        public SectionFeedbackViewModel? TeachingRoomsFeedback { get; set; }
        public SectionFeedbackViewModel? LaboratoryFeedback { get; set; }
        public SectionFeedbackViewModel? MuseumFeedback { get; set; }

        // Replace object with the actual return type
        // of GetTabSectionFeedbackAsync().
        public object? SectionFeedback { get; set; }
    }
}
