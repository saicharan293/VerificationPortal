namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface ITabService
    {
        string GetDatabaseTabName(string tabName);
        Task<int?> GetTabIdAsync(string tabName, int facultyId);
    }
}
