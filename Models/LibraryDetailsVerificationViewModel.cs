namespace VerificationPortal.Models
{
    public class LibraryDetailsVerificationViewModel
    {
        public string? CollegeCode { get; set; }

        public int FacultyCode { get; set; }

        public int AffiliationType { get; set; }

        public LibraryGeneralVerification? General { get; set; }

        public List<LibraryItemVerificationRow> Items { get; set; } = new();

        public LibraryBuildingVerification? Building { get; set; }

        public List<LibraryTechnicalProcessVerificationRow> TechnicalProcesses { get; set; } = new();

        public List<LibraryEquipmentVerificationRow> Equipments { get; set; } = new();
        public LibraryFinanceVerification? Finance { get; set; }
    }

    public class LibraryGeneralVerification
    {
        public string? LibraryEmailId { get; set; }

        public string? DigitalLibrary { get; set; }

        public string? HelinetServices { get; set; }

        public string? DepartmentWiseLibrary { get; set; }
    }

    public class LibraryItemVerificationRow
    {
        public int SlNo { get; set; }

        public string? ItemName { get; set; }

        public int? CurrentForeign { get; set; }

        public int? CurrentIndian { get; set; }

        public int? PreviousForeign { get; set; }

        public int? PreviousIndian { get; set; }
    }

    public class LibraryBuildingVerification
    {
        public string? IsIndependent { get; set; }

        public decimal? AreaSqMtrs { get; set; }
    }

    public class LibraryTechnicalProcessVerificationRow
    {
        public int SlNo { get; set; }

        public string? ProcessName { get; set; }

        public string? Value { get; set; }
    }

    public class LibraryEquipmentVerificationRow
    {
        public int SlNo { get; set; }

        public string? EquipmentName { get; set; }

        public string? HasEquipment { get; set; }
    }
    public class LibraryFinanceVerification
    {
        public decimal? TotalBudgetLakhs { get; set; }

        public decimal? ExpenditureBooksLakhs { get; set; }
    }
}
