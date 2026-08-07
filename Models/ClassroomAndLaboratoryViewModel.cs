using VerificationPortal.ViewModels;

namespace VerificationPortal.Models
{
    public class ClassroomAndLaboratoryViewModel
    {
        public string CollegeCode { get; set; }

        public int FacultyCode { get; set; }

        public int SeatIntake { get; set; }

        public int SeatSlab { get; set; }

        public MedicalSkillsLaboratory? MedicalSkillsLaboratory { get; set; }

        //public List<DentalPreClinicalAndSkillsLabAreaReq> PreClinicalLabRequirements { get; set; }
            = new();

        public List<DentalInfrastructureVM> InfrastructureDetails { get; set; }
            = new();

        public DentalCollegeLandBuildingDetail? LandBuildingDetails { get; set; }
    }
}
