
using VerificationPortal.Models;

namespace VerificationPortal.ViewModels
{
    public class DocumentManagerViewModel
    {
        // ============================================================
        // DOCUMENT FORM
        // Stores the values entered by the admin
        // ============================================================

        // Selected Faculty
        public int FacultyId { get; set; }

        // Selected Tab
        public int TabId { get; set; }

        // Selected Section
        // Nullable because a document may belong directly to a Tab
        public int? SectionId { get; set; }

        // Name of the document
        // Example: Fire Safety Certificate
        public string DocumentName { get; set; } = string.Empty;

        // Indicates whether the document is mandatory
        // Default value is true
        public bool IsMandatory { get; set; } = true;

        // Controls the display order of the document
        public int? DisplayOrder { get; set; }


        // ============================================================
        // DROPDOWN DATA
        // Used to populate Faculty, Tab, and Section dropdowns
        // ============================================================

        // List of faculties
        public List<Faculty> Faculties { get; set; } = new();

        // List of tabs
        // Initially empty and loaded based on selected Faculty
        public List<MstTab> Tabs { get; set; } = new();

        // List of sections
        // Initially empty and loaded based on selected Tab
        public List<MstSection> Sections { get; set; } = new();


        // ============================================================
        // DOCUMENT LIST
        // Stores all documents already added to MstDocument
        // ============================================================

        public List<DocumentListViewModel> Documents { get; set; } = new();
    }

    public class DocumentListViewModel
    {
        public int DocumentId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public int FacultyId { get; set; }
        public string TabName { get; set; } = string.Empty;
        public int TabId { get; set; }
        public int? SectionId { get; set; }

        public string? SectionName { get; set; }
        public string DocumentName { get; set; } = string.Empty;

        public bool IsMandatory { get; set; }
        public int? DisplayOrder { get; set; }

    }
}
