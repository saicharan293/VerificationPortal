using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class DocumentViewerService : IDocumentViewerService
    {
        private readonly IWebHostEnvironment _environment;

        public DocumentViewerService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string? ResolveDocumentPath(string? storedPath)
        {
            if (string.IsNullOrWhiteSpace(storedPath))
                return null;

            var filePath = Path.IsPathRooted(storedPath) ? storedPath : Path.Combine(_environment.ContentRootPath, storedPath);

            return filePath;
        }

        public bool DocumentExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        public string GetDocumentContentType(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "applicaiton/octet-stream"
            };
        }
    }
}
