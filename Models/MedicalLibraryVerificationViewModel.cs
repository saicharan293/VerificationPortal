namespace VerificationPortal.Models
{
    public class MedicalLibraryVerificationViewModel
    {
        public string? CollegeCode { get; set; }

        public int FacultyCode { get; set; }

        public int AffiliationType { get; set; }

        public string? CourseLevel { get; set; }

        public List<LibraryServiceVerificationRow> LibraryServices { get; set; }
            = new();

        public string? UsageReportFileName { get; set; }

        public List<LibraryStaffVerificationRow> LibraryStaff { get; set; }
            = new();

        public List<DepartmentLibraryVerificationRow> DepartmentLibraries { get; set; }
            = new();

        public MedicalLibraryOtherVerification? OtherDetails { get; set; }

        public List<DentalLibraryVerificationRow> DentalLibraryRecords { get; set; }
            = new();
    }

    public class LibraryServiceVerificationRow
    {
        public int ServiceId { get; set; }

        public string ServiceName { get; set; } = "";

        public string? IsAvailable { get; set; }

        public string? UploadedFileName { get; set; }
    }

    public class LibraryStaffVerificationRow
    {
        public string? StaffName { get; set; }

        public string? Designation { get; set; }

        public string? Qualification { get; set; }

        public int Experience { get; set; }

        public string? Category { get; set; }
    }

    public class DepartmentLibraryVerificationRow
    {
        public string? DepartmentCode { get; set; }

        public int TotalBooks { get; set; }

        public int BooksAddedInYear { get; set; }

        public int CurrentJournals { get; set; }

        public string? LibraryStaff1 { get; set; }

        public string? LibraryStaff2 { get; set; }

        public int? Titles { get; set; }

        public int? InternationalJournals { get; set; }

        public int? BackVolumes { get; set; }

        public int? PrintJournalPercentage { get; set; }
    }

    public class MedicalLibraryOtherVerification
    {
        public string? HasDigitalValuationCentre { get; set; }

        public int? NoOfSystems { get; set; }

        public string? HasStableInternet { get; set; }

        public string? HasCccameraSystem { get; set; }

        public string? SpecialFeaturesQuestion { get; set; }

        public string? UploadedFileName { get; set; }
    }

    public class DentalLibraryVerificationRow
    {
        public int RecordId { get; set; }

        public string? RecordName { get; set; }

        public string? ExistingFileName { get; set; }
    }
}