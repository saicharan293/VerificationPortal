using Microsoft.EntityFrameworkCore;
using VerificationPortal.Application.Interfaces;
using VerificationPortal.DATA;
using VerificationPortal.Models;

namespace VerificationPortal.Services.Verification.Services
{
    public class CollegeDesignationService : ICollegeDesignationService
    {
        private readonly ApplicationDbContext _context;

        public CollegeDesignationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CollegeDesignationItemVm>> GetCollegeDesignationsAsync(string collegeCode, int facultyId)
        {
            var designations = await _context.CollegeDesignationDetails
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.FacultyCode == facultyId.ToString())
                .Select(x => new CollegeDesignationItemVm
                {
                    Id = x.Id,
                    FacultyCode = x.FacultyCode,
                    CollegeCode = x.CollegeCode,
                    Designation = x.Designation,
                    DesignationCode = x.DesignationCode,
                    Department = x.Department,
                    DepartmentCode = x.DepartmentCode,
                    SeatSlabId = x.SeatSlabId,
                    RequiredIntake = x.RequiredIntake,
                    AvailableIntake = x.AvailableIntake,

                    // Additional properties
                    // Map these if the corresponding entity properties exist.
                })
                .OrderBy(x => x.Department)
                .ThenBy(x => x.Designation)
                .ToListAsync();

            return designations;
        }
    }
}
