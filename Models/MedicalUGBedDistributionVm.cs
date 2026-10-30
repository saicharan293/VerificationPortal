namespace VerificationPortal.Models
{
    public class MedicalUGBedDistributionVm
    {
        public VerificationPageContextVm PageContext { get; set; } = new();
        public int Id { get; set; }

        // =========================
        // MEDICAL
        // =========================

        public int? OralMaxillofacialSurgery { get; set; } 
        public int? GenMedicine { get; set; }
        public int? Paediatrics { get; set; }
        public int? SkinVD { get; set; }
        public int? Psychiatry { get; set; }


        public int? GenSurgery { get; set; }
        public int? Orthopaedics { get; set; }
        public int? Ophthalmology { get; set; }
        public int? ENT { get; set; }

        public int? ObstetricsANC { get; set; }
        public int? Gynaecology { get; set; }
        public int? Postpartum { get; set; }

        public int? MajorOT { get; set; }
        public int? MinorOT { get; set; }

        public int? ICCU { get; set; }
        public int? ICU { get; set; }
        public int? PICU_NICU { get; set; }
        public int? SICU { get; set; }
        public int? TotalICUBeds { get; set; }
        public int? CasualtyBeds { get; set; }

        // =========================================
        // SECTION TOTALS
        // =========================================

        public int MedicineTotal =>
            (GenMedicine ?? 0) +
            (Paediatrics ?? 0) +
            (SkinVD ?? 0) +
            (Psychiatry ?? 0);

        public int SurgeryTotal =>
            (GenSurgery ?? 0) +
            (Orthopaedics ?? 0) +
            (Ophthalmology ?? 0) +
            (ENT ?? 0);

        public int ObstetricsGynaecologyTotal =>
            (ObstetricsANC ?? 0) +
            (Gynaecology ?? 0) +
            (Postpartum ?? 0);

        public int ClinicalTotal =>
            MedicineTotal +
            SurgeryTotal +
            ObstetricsGynaecologyTotal;

        public int OperationTheatreTotal =>
            (MajorOT ?? 0) +
            (MinorOT ?? 0);

        public int IntensiveCareTotal =>
            (ICCU ?? 0) +
            (ICU ?? 0) +
            (PICU_NICU ?? 0) +
            (SICU ?? 0);

        public int DentalTotal => OralMaxillofacialSurgery ?? 0;
        public List<DentalWardBedDistributionVm> DentalWards { get; set; } = new();

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; } = new();
    }

    

}