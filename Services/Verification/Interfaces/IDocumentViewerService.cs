namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface IDocumentViewerService
    {
        string? ResolveDocumentPath(string? storedPath);
        bool DocumentExists(string filePath);
        string GetDocumentContentType(string filePath);
    }
}
