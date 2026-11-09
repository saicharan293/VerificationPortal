using VerificationPortal.Models;

namespace VerificationPortal.Application.Interfaces
{
    public interface ICollegeDesignationService
    {
        Task<List<CollegeDesignationItemVm>> GetCollegeDesignationsAsync(string collegeCode, int facultyId);
    }
}