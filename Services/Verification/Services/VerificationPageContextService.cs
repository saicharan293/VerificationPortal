using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class VerificationPageContextService : IVerificationPageContextService
    {
        private readonly ApplicationDbContext _context;

        public VerificationPageContextService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationPageContext> GetPageContextAsync (string collegeCode, string userId)
        {
            var institution = await _context.AffInstitutionsDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.CollegeCode == collegeCode);

            if (institution == null)
                throw new Exception($"Institution not found for the college code '{collegeCode}");

            var faculty = await _context.Faculties
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.FacultyId.ToString() == institution.FacultyCode);

            if (faculty == null) throw new Exception("Faculty Not found.");

            if (string.IsNullOrWhiteSpace(userId)) throw new Exception("User session expired. Pleae login again.");

            var user = await _context.TblRguhsFacultyUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId.ToString() == userId);

            if (user == null) throw new Exception("Logged-in user not found.");

            return new VerificationPageContext
            {
                Institution = institution,
                Faculty = faculty,
                User = user
            };
        }
    }
}
