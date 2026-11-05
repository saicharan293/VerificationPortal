namespace VerificationPortal.Models
{
    public class ClinicalFacilitiesVerificationViewModel
    {
        public string? CollegeCode { get; set; }
        public int FacultyCode { get; set; }
        public int AffiliationTypeId { get; set; }

        public ClinicalHospitalVerificationRow HospitalDetails { get; set; }
            = new();

        public ClinicalCapacityFormVM ClinicalStatistics { get; set; }
        public List<DisciplineVm> Disciplines { get; set; } = new();

        public NPTARequirementsPostVM NptaRequirementPostvm { get; set; }  = new();

        public EngAlliedRequirementsPostVM EngAlliedRequirementPostvm { get; set; } = new();

        public List<DentalWardBedDistributionVm> DentalWardBedDistribution { get; set; } = new();

        public HospitalCertificatesVm HospitalCertificates { get; set; } = new();

        public AnatomyActRegistrationVm AnatomyActRegistration { get; set; }  = new();
        public HospitalTieUpVm HospitalTieUp { get; set; } = new();
        public HospitalMedicalCertificatesVm HospitalMedicalCertificates {get; set;} = new();

        // Indoor Infrastructure Verification
        public IndoorDepartmentRequirementsVerificationVM IndoorDepartmentRequirements { get; set; } = new();

        public OTRequirementsVerificationVM OTRequirements { get; set; } = new();

        public CasualityRequirementsVerificationVM CasualityRequirements { get; set; } = new();

        public CSSDandLaundryRequirementsVerificationVM CSSDandLaundryRequirements { get; set; } = new();

        public RadioDiagnosisRequirementsVerificationVM RadioDiagnosisRequirements { get; set; } = new();

        public AnaesthesiologyRequirementsVerificationVM AnaesthesiologyRequirements { get; set; } = new();

        public CentralLaboratoryRequirementsVerificationVM CentralLaboratoryRequirements { get; set; } = new();

        public BloodBankRequirementsVerificationVM BloodBankRequirements { get; set; } = new();

        public YogaRequirementsVerificationVM YogaRequirements { get; set; } = new();

        public RadiationOncologyRequirementsVerificationVM RadiationOncologyRequirements { get; set; } = new();

        public ArtCenterRequirementsVerificationVM ArtCenterRequirements { get; set; } = new();

        public PharmacyRequirementsVerificationVM PharmacyRequirements { get; set; } = new();

        public UtilitiesRequirementsVerificationVM UtilitiesRequirements { get; set; } = new();

        public IndoorBedsUnitsRequirementsVerificationVM IndoorBedsUnitsRequirements { get; set; } = new();

        public OutpatientAreaRequirementsVerificationVM OutpatientAreaRequirements { get; set; } = new();

        public SuperVisionInFieldPracticeAreaVerificationVM SuperVisionInFieldPracticeArea { get; set; } = new();

        public IndoorBedsRequirementsVerificationVM IndoorBedsRequirements { get; set; } = new();

    }

    public class HospitalMedicalCertificatesVm
    {
        public List<AffiliatedHospitalDocumentVm> Documents { get; set; } = new();
    }

    public class AffiliatedHospitalDocumentVm
    {
        public int DocumentId { get; set; }

        public string? CollegeCode { get; set; }

        public string? HospitalType { get; set; }

        public string? HospitalName { get; set; }

        public int? TotalBeds { get; set; }

        public string? DocumentName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? CourseLevel { get; set; }

        public string? DocumentFilePath { get; set; }
    }

    public class ClinicalHospitalVerificationRow
    {
        public int HospitalDetailsId { get; set; }

        public string? CollegeCode { get; set; }
        public string? FacultyCode { get; set; }

        public string? CourseLevel { get; set; }

        public string? AffiliationType { get; set; }

        public bool? ParentMedicalCollegeExists { get; set; }

        public string? HospitalType { get; set; }

        public string? HospitalOwnedBy { get; set; }

        public string? HospitalOwnerName { get; set; }

        public string? HospitalName { get; set; }

        public string? HospitalDistrict { get; set; }

        public string? HospitalTaluk { get; set; }

        public string? Location { get; set; }

        public bool? IsParentHospitalForOtherNursingInstitution { get; set; }

        public bool SupportingDocumentExists { get; set; }

        public List<HospitalFacilityVerificationRow> Facilities { get; set; }
            = new();
    }


    public class HospitalFacilityVerificationRow
    {
        public int FacilityId { get; set; }

        public string? FacilityName { get; set; }

        public bool IsSelected { get; set; }
    }

    public class HospitalCertificatesVm
    {
        public VerificationPageContext PageContext { get; set; } = new();
        public int? KPMECertificateFileId { get; set; }

        public string? KPMECertificatePath { get; set; }


        public int? PollutionControlBoardCertificateFileID { get; set; }

        public string? PollutionControlBoardCertificatePath { get; set; }


        public int? BioMedicalCertificateFileId { get; set; }

        public string? BioMedicalCertificatePath { get; set; }


        public int? DrugFreeCampusCertificationFileId { get; set; }

        public string? DrugFreeCampusCertificationPath { get; set; }


        public int? ProposedPlansForFutureDevelopmentsId { get; set; }

        public string? ProposedPlansForFutureDevelopmentsPath { get; set; }
    }

    public class AnatomyActRegistrationVm
    {
        public VerificationPageContext PageContext { get; set; } = new();
        public bool? HasAnatomyActRegistration { get; set; }

        public string? AnatomyActRegistrationDetails { get; set; }

        public int? AnatomyActRegistrationPdfFileId { get; set; }

        public string? AnatomyActRegistrationPdfPath { get; set; }
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }
    public class HospitalTieUpVm
    {
        public VerificationPageContext PageContext { get; set; } = new();
        public bool? HasHospitalTieUp { get; set; }

        public List<HospitalTieUpDetailVM> HospitalTieUps { get; set; } = new();
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();

    }

    public class HospitalTieUpDetailVM
    {
        public int Id { get; set; }

        public int HospitalDetailsId { get; set; }

        public string CollegeCode { get; set; } = string.Empty;

        public int FacultyCode { get; set; }

        public string CourseLevel { get; set; } = string.Empty;

        public string? TieUpType { get; set; }

        public string? HospitalName { get; set; }

        public string? HospitalAddress { get; set; }

        public string? TieUpDetails { get; set; }

        public int? SupportingDocumentFileId { get; set; }

        public string? SupportingDocumentPath { get; set; }

        public string? SupportingDocumentName { get; set; }

        public string? SupportingDocumentContentType { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
    public class RequirementVerificationBaseVM
    {
        public int RequirementId { get; set; }

        public string RequirementName { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }

        public string? Remarks { get; set; }

        public bool IsVerified { get; set; }
    }

    public class IndoorDepartmentRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class OTRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class CasualityRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class CSSDandLaundryRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class RadioDiagnosisRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class AnaesthesiologyRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class CentralLaboratoryRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class BloodBankRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class YogaRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class RadiationOncologyRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class ArtCenterRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class PharmacyRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class UtilitiesRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class IndoorBedsUnitsRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class OutpatientAreaRequirementVerificationVM : RequirementVerificationBaseVM { }

    public class IndoorInfrastructureVerificationBaseVM
    {
        public string? CollegeCode { get; set; }

        public int HospitalDetailsId { get; set; }

        public int FacultyCode { get; set; }

        public int AffiliationTypeId { get; set; }

        public string? CourseLevel { get; set; }
    }

    public class IndoorDepartmentRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<IndoorDepartmentRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class OTRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<OTRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class CasualityRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<CasualityRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class CSSDandLaundryRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<CSSDandLaundryRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class RadioDiagnosisRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<RadioDiagnosisRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class AnaesthesiologyRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<AnaesthesiologyRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class CentralLaboratoryRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<CentralLaboratoryRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class BloodBankRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<BloodBankRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class YogaRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<YogaRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class RadiationOncologyRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<RadiationOncologyRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class ArtCenterRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<ArtCenterRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class PharmacyRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<PharmacyRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class UtilitiesRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<UtilitiesRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class IndoorBedsUnitsRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<IndoorBedsUnitsRequirementVerificationVM> Requirements { get; set; } = new();
    }

    public class OutpatientAreaRequirementsVerificationVM : IndoorInfrastructureVerificationBaseVM
    {
        public List<OutpatientAreaRequirementVerificationVM> Requirements { get; set; } = new();
    }


    public class SuperVisionInFieldPracticeAreaVerificationVM
    {
        public string? CollegeCode { get; set; }

        public int FacultyCode { get; set; }

        public int AffiliationTypeId { get; set; }

        public int HospitalDetailsId { get; set; }

        public string? CourseLevel { get; set; }

        public List<SuperVisionInFieldPracticeAreaVerificationItemVM> Supervisors { get; set; }
            = new();
    }

    public class SuperVisionInFieldPracticeAreaVerificationItemVM
    {
        public int Id { get; set; }

        public string? Post { get; set; }

        public string? Name { get; set; }

        public string? Qualification { get; set; }

        public DateOnly? YearOfQualification { get; set; }

        public string? University { get; set; }

        public DateOnly? UgFromDate { get; set; }

        public DateOnly? UgToDate { get; set; }

        public DateOnly? PgFromDate { get; set; }

        public DateOnly? PgToDate { get; set; }

        public string? Responsibilities { get; set; }
    }

    public class IndoorBedsRequirementsVerificationVM
    {
        public string? CollegeCode { get; set; }

        public int FacultyCode { get; set; }

        public int AffiliationTypeId { get; set; }

        public string? CourseLevel { get; set; }

        public List<IndoorBedsOccupancyItemVM> Items { get; set; } = new();
    }
}