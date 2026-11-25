using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using System.Security.Claims;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Controllers
{
    [Authorize(Roles = "Admin,ViceChancellor,Director")]
    public class CollegeMappingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CollegeMappingController> _logger;
        private readonly ICollegeMappingService _collegeMappingService;

        public CollegeMappingController(
            ApplicationDbContext context,
            ICollegeMappingService collegeMappingService,
            ILogger<CollegeMappingController> logger)
        {
            _context = context;
            _collegeMappingService = collegeMappingService;
            _logger = logger;
        }

        // GET: /CollegeMapping
        public async Task<IActionResult> Index(string? search, int? facultyId)
        {
            
            var model = await _collegeMappingService.GetMappingsAsync(search, facultyId);

            var faculties = await _context.Faculties.Where(e => e.Status == "Active").OrderBy(f => f.FacultyName).ToListAsync();


            ViewBag.Faculties = faculties;
            ViewBag.Search = search;
            ViewBag.SelectedFaculty = facultyId;

            return View(model);
        }

        // GET: /CollegeMapping/Create
        // [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Create(int? facultyId)
        {
            var model = await _collegeMappingService.GetCreateViewModelAsync(facultyId);

            return View(model);
        }

        // POST: /CollegeMapping/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Create(CollegeMappingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _collegeMappingService.PopulateCreateViewModelAsync(model);
                return View(model);
            }

            // 2. Call service
            var errorMessage = await _collegeMappingService.CreateMappingAsync(model, User.Identity?.Name ?? "System");

            // 3. Handle business validation errors
            if (errorMessage != null)
            {
                ModelState.AddModelError("", errorMessage);

                await _collegeMappingService.PopulateCreateViewModelAsync(model);

                return View(model);
            }

            // 4. Success
            TempData["SuccessMessage"] = "College mapping created successfully.";

            return RedirectToAction(nameof(Index));
        }

        

        // GET: /CollegeMapping/Edit/5
        // [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var model = await _collegeMappingService.GetEditViewModelAsync(id.Value);

            if (model == null) return NotFound();

            return View(model);
        }

        // POST: /CollegeMapping/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Edit(int id, CollegeMappingEditViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await _collegeMappingService.PopulateEditViewModelAsync(model);
                return View(model);
            }

            try
            {
                var updated = await _collegeMappingService.UpdateMappingAsync(id, model, User.Identity?.Name ?? "");

                if (!updated)
                    return NotFound();

                // Keep session in sync with the selected verification level
                if (id > 0)
                {
                    var verificationLevel = model.NextVerificationLevel ?? model.VerificationLevel;

                    HttpContext.Session.SetInt32("VerificationLevel", verificationLevel);
                }

                TempData["SuccessMessage"] = "College mapping updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _collegeMappingService.MappingExists(model.Id))
                    return NotFound();

                throw;
            }
        }


        // POST: /CollegeMapping/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            var deleted = await _collegeMappingService.DeleteMappingAsync(id);

            if (!deleted)
                return NotFound();

            TempData["SuccessMessage"] = "College mapping deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /CollegeMapping/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var mapping = await _context.TblCollegeMappings.FindAsync(id);

            if (mapping == null)
            {
                return Json(new { success = false });
            }
            mapping.IsActive = !mapping.IsActive;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                isActive = mapping.IsActive
            });
        }

        // GET: /CollegeMapping/GetCollegesByMapping/5
        [HttpGet]
        // [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> GetCollegesByMapping(int id)
        {
            var mapping = await _context.TblCollegeMappings
                .Include(m => m.FacultyCodeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mapping == null)
            {
                return NotFound();
            }

            return await GetCollegesByUserAndFaculty(mapping.UserId, mapping.FacultyCode);
        }

        // GET: /CollegeMapping/GetCollegesByUserFaculty?userId=X&facultyCode=Y
        [HttpGet]
        // [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> GetCollegesByUserFaculty(int userId, int facultyCode)
        {
            return await GetCollegesByUserAndFaculty(userId, facultyCode);
        }

        private async Task<IActionResult> GetCollegesByUserAndFaculty(int userId, int facultyCode)
        {
            // Get all mappings for this user+faculty to determine the combined range
            var mappings = await _context.TblCollegeMappings
                .Include(m => m.FacultyCodeNavigation)
                .Where(m => m.UserId == userId && m.FacultyCode == facultyCode)
                .ToListAsync();

            if (!mappings.Any())
            {
                return NotFound();
            }

            var firstMapping = mappings.First();

            // Get combined range from TblAllCollegesMapping
            var combinedFrom = mappings
                .Where(m => m.AllCollegesMapping != null)
                .Select(m => m.AllCollegesMapping!.FromLetter)
                .OrderBy(x => x)
                .FirstOrDefault();

            var combinedTo = mappings
                .Where(m => m.AllCollegesMapping != null)
                .Select(m => m.AllCollegesMapping!.ToLetter)
                .OrderByDescending(x => x)
                .FirstOrDefault();

            var query = _context.AffiliationCollegeMasters
                .Where(c => c.FacultyCode == facultyCode.ToString() && c.Status == true);

            // Apply alphabetical range filter based on combined CollegeFrom and CollegeTo
            if (!string.IsNullOrWhiteSpace(combinedFrom))
            {
                query = query.Where(c => c.CollegeName != null &&
                    string.Compare(c.CollegeName.Substring(0, 1).ToUpper(), combinedFrom.ToUpper()) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(combinedTo))
            {
                query = query.Where(c => c.CollegeName != null &&
                    string.Compare(c.CollegeName.Substring(0, 1).ToUpper(), combinedTo.ToUpper()) <= 0);
            }

            var colleges = await query
                .OrderBy(c => c.CollegeName)
                .Select(c => new
                {
                    code = c.CollegeCode,
                    name = c.CollegeName ?? "",
                    town = c.CollegeTown ?? ""
                })
                .ToListAsync();

            return Json(new
            {
                userId = userId,
                facultyCode = facultyCode,
                userName = firstMapping.UserName,
                facultyName = firstMapping.FacultyCodeNavigation?.FacultyName,
                collegeFrom = combinedFrom,
                collegeTo = combinedTo,
                colleges = colleges,
                totalCount = colleges.Count
            });
        }

        // POST: /CollegeMapping/UpdateVerificationLevels
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> UpdateVerificationLevels([FromBody] List<int> orderedIds)
        {
            var mappings = await _context.TblCollegeMappings
                .Where(m => orderedIds.Contains(m.Id))
                .ToListAsync();

            for (int i = 0; i < orderedIds.Count; i++)
            {
                var mapping = mappings.FirstOrDefault(m => m.Id == orderedIds[i]);
                if (mapping != null)
                    mapping.VerificationLevel = i + 1;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ==================== HELPER METHODS ====================


        private async Task<List<TblRguhsFacultyUser>> GetNonAdminUsersAsync(int? facultyId)
        {
            var query = _context.TblRguhsFacultyUsers
                .Where(u => u.IsActive == true)
                .AsQueryable();

            // Exclude system admins (no faculty assigned)
            if (facultyId.HasValue && facultyId.Value > 0)
            {
                query = query.Where(u => u.Faculty == facultyId.Value);
            }
            else
            {
                query = query.Where(u => u.Faculty != null && u.Faculty > 0);
            }

            return await query
                .OrderBy(u => u.Faculty)
                .ThenBy(u => u.UserName)
                .ToListAsync();
        }

       
        // GET: /CollegeMapping/GetCollegesByFaculty?mappingId=X
        [HttpGet]
        public async Task<IActionResult> GetCollegesByFaculty(int mappingId)
        {
            // Get the mapping details

            var colleges = await _collegeMappingService.GetCollegesByFacultyAsync(mappingId);

            if (colleges == null) return NotFound("College mapping not found or inactive.");

            return Json(colleges.Select(c => new
            {
                code = c.Code,
                name = c.Name,
                town = c.Town
            }));
        }

        [HttpGet]
        public async Task<IActionResult> GetCollegesByFacultyId( int facultyId, string fromLetter, string toLetter)
        {
            if (facultyId <= 0 ||
                string.IsNullOrWhiteSpace(fromLetter) ||
                string.IsNullOrWhiteSpace(toLetter))
            {
                return BadRequest("Invalid faculty or college name range.");
            }

            fromLetter = fromLetter.Trim().ToUpperInvariant();
            toLetter = toLetter.Trim().ToUpperInvariant();

            if (fromLetter.Length != 1 ||
                toLetter.Length != 1 ||
                fromLetter[0] > toLetter[0])
            {
                return BadRequest("Invalid college name range.");
            }

            var colleges = await _collegeMappingService.GetCollegesByFacultyIdAsync(facultyId, fromLetter, toLetter);

            return Json(colleges.Select(c => new
            {
                code = c.Code,
                name = c.Name,
                town = c.Town
            }));
        }

        private async Task PopulateRangeViewModel(CollegeMappingRangeViewModel model)
        {
            model.AvailableUsers = await GetNonAdminUsersAsync(model.SelectedFacultyId);
            model.AvailableFaculties = await _context.Faculties
                .OrderBy(f => f.FacultyName)
                .ToListAsync();
        }
    }
}
