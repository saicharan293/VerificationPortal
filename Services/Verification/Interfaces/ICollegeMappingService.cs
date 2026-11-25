using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Interfaces
{
    public interface ICollegeMappingService
    {
        Task<CollegeMappingListViewModel> GetMappingsAsync(string? search, int? facultyId);
        Task<CollegeMappingCreateViewModel> GetCreateViewModelAsync(int? facultyId);

        Task<string?> CreateMappingAsync(CollegeMappingCreateViewModel model, string createdBy);
        Task PopulateCreateViewModelAsync(CollegeMappingCreateViewModel model);

        Task<bool>  MappingExists(int id);

        Task<CollegeMappingEditViewModel?> GetEditViewModelAsync(int id);

        Task PopulateEditViewModelAsync(CollegeMappingEditViewModel model);

        Task<bool> UpdateMappingAsync(int id, CollegeMappingEditViewModel model, string modifiedBy);
        Task<bool> DeleteMappingAsync(int id);

        Task<List<CollegeOptionViewModel>> GetCollegesByFacultyAsync(int mappingId);
        Task<List<CollegeOptionViewModel>> GetCollegesByFacultyIdAsync(int facultyId, string fromLetter, string toLetter);
    }
}
