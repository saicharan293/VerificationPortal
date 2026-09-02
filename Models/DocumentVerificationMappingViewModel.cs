namespace VerificationPortal.Models
{
    public class DocumentVerificationMappingViewModel
    {
        // Master document ID from MstDocument
        public int DocumentId { get; set; }

        // Name configured in MstDocument
        public string DocumentName { get; set; } = string.Empty;
    }
}
