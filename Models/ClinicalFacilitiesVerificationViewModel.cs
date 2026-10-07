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
}