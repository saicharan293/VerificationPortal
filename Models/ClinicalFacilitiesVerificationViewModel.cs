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
}