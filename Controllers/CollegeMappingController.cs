using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Models.ViewModels;

namespace VerificationPortal.Controllers
{
    [Authorize]
    public class CollegeMappingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CollegeMappingController> _logger;

        public CollegeMappingController(
            ApplicationDbContext context,
            ILogger<CollegeMappingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /CollegeMapping
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Index(string? search, int? facultyId)
        {
            var query = _context.TblCollegeMappings
                .Include(m => m.FacultyCodeNavigation)
                .AsQueryable();

            // Filter by faculty
            if (facultyId.HasValue && facultyId.Value > 0)
            {
                query = query.Where(m => m.FacultyCode == facultyId.Value);
            }

            // Search by username
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.UserName.Contains(search));
            }

            var mappings = await query
                .OrderBy(m => m.FacultyCode)
                .ThenBy(m => m.UserName)
                .ToListAsync();

            // Get user details
            var userIds = mappings.Select(m => m.UserId).Distinct().ToList();
            var users = await _context.TblRguhsFacultyUsers
                .Where(u => userIds.Contains(u.UserId))
                .ToListAsync();

            // Group by UserId + FacultyCode to show one row per user per faculty
            var groupedMappings = mappings
                .GroupBy(m => new { m.UserId, m.FacultyCode })
                .Select(g => new
                {
                    UserId = g.Key.UserId,
                    FacultyCode = g.Key.FacultyCode,
                    UserName = g.First().UserName,
                    FromLetter = g.Min(m => m.FromLetter),
                    ToLetter = g.Max(m => m.ToLetter),

                    CollegeFrom = g.Min(m => m.CollegeFrom),
                    CollegeTo = g.Max(m => m.CollegeTo),

                    IsActive = g.All(m => m.IsActive == true) ? true : false,
                    CreatedDate = g.Min(m => m.CreatedDate),
                    CreatedBy = g.First().CreatedBy,
                    FacultyName = g.First().FacultyCodeNavigation?.FacultyName,
                    UserDesignation = users.FirstOrDefault(u => u.UserId == g.Key.UserId)?.DesignationDescription,
                    MappingCount = g.Count(),
                    MappingIds = g.Select(m => m.Id).ToList()
                })
                .ToList();

            var mappingList = groupedMappings.Select(g => new CollegeMappingWithUser
            {
                Mapping = new TblCollegeMapping
                {
                    Id = g.MappingIds.First(), // Use first mapping ID as representative
                    UserId = g.UserId,
                    UserName = g.UserName,
                    FacultyCode = g.FacultyCode,
                    FromLetter = g.FromLetter,
                    ToLetter = g.ToLetter,
                    CollegeFrom = g.CollegeFrom,
                    CollegeTo = g.CollegeTo,
                    IsActive = g.IsActive,
                    CreatedDate = g.CreatedDate,
                    CreatedBy = g.CreatedBy
                },
                UserDesignation = g.UserDesignation,
                FacultyName = g.FacultyName
            }).ToList();

            var faculties = await _context.Faculties.OrderBy(f => f.FacultyName).ToListAsync();

            var model = new CollegeMappingListViewModel
            {
                Mappings = mappingList,
                TotalMappings = await _context.TblCollegeMappings.CountAsync(),
                ActiveMappings = await _context.TblCollegeMappings.CountAsync(m => m.IsActive == true),
                TotalUsers = await _context.TblCollegeMappings.Select(m => m.UserId).Distinct().CountAsync()
            };

            ViewBag.Faculties = faculties;
            ViewBag.Search = search;
            ViewBag.SelectedFaculty = facultyId;

            return View(model);
        }

        // GET: /CollegeMapping/Create
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Create(int? facultyId)
        {
            var model = new CollegeMappingCreateViewModel
            {
                AvailableUsers = await GetNonAdminUsersAsync(facultyId),
                AvailableFaculties = await _context.Faculties
                    .OrderBy(f => f.FacultyName)
                    .ToListAsync()
            };

            if (facultyId.HasValue && facultyId.Value > 0)
            {
                model.AvailableColleges = await _context.AffiliationCollegeMasters
                    .Where(c => c.FacultyCode == facultyId.Value.ToString() && c.Status == true)
                    .OrderBy(c => c.CollegeName)
                    .ToListAsync();
            }

            return View(model);
        }

        // POST: /CollegeMapping/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Create(CollegeMappingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateViewModel(model);
                return View(model);
            }

            // Check for duplicate mapping
            var existingMapping = await _context.TblCollegeMappings
                .FirstOrDefaultAsync(m => m.UserId == model.SelectedUserId
                                       && m.FacultyCode == model.SelectedFacultyId);

            if (existingMapping != null)
            {
                ModelState.AddModelError("",
                    "This user already has a college mapping. Please edit the existing one or delete it first.");
                await PopulateCreateViewModel(model);
                return View(model);
            }

            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.Id == model.SelectedUserId);

            if (user == null)
            {
                ModelState.AddModelError("", "Selected user not found.");
                await PopulateCreateViewModel(model);
                return View(model);
            }

            // Get selected colleges to determine the alphabetical range
            var selectedColleges = await _context.AffiliationCollegeMasters
                .Where(c => model.SelectedCollegeCodes.Contains(c.CollegeCode))
                .OrderBy(c => c.CollegeName)
                .ToListAsync();

            if (!selectedColleges.Any())
            {
                ModelState.AddModelError("", "No valid colleges selected.");
                await PopulateCreateViewModel(model);
                return View(model);
            }

            // Determine the alphabetical range from college names
            var firstLetter = selectedColleges.First().CollegeName?.Trim().Substring(0, 1).ToUpper() ?? "A";
            var lastLetter = selectedColleges.Last().CollegeName?.Trim().Substring(0, 1).ToUpper() ?? "Z";

            // Create a SINGLE mapping with the range
            var mapping = new TblCollegeMapping
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FacultyCode = model.SelectedFacultyId,

                // Alphabetical Range
                FromLetter = firstLetter,
                ToLetter = lastLetter,

                // College Code Range
                CollegeFrom = selectedColleges.First().CollegeCode,
                CollegeTo = selectedColleges.Last().CollegeCode,

                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "System",
                IsActive = true
            };

            _context.TblCollegeMappings.Add(mapping);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Admin {Admin} assigned range {From}-{To} ({Count} colleges) to user {UserId} ({UserName})",
                User.Identity?.Name, firstLetter, lastLetter, selectedColleges.Count, user.UserId, user.UserName);

            TempData["SuccessMessage"] =
                $"Successfully assigned {selectedColleges.Count} college(s) ({firstLetter}-{lastLetter}) to {user.UserName}.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /CollegeMapping/AssignByRange
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> AssignByRange(int? facultyId)
        {
            var model = new CollegeMappingRangeViewModel
            {
                AvailableUsers = await GetNonAdminUsersAsync(facultyId),
                AvailableFaculties = await _context.Faculties
                    .OrderBy(f => f.FacultyName)
                    .ToListAsync()
            };
            return View(model);
        }

        // POST: /CollegeMapping/AssignByRange
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> AssignByRange(CollegeMappingRangeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRangeViewModel(model);
                return View(model);
            }

            if (model.CollegeFrom.CompareTo(model.CollegeTo) > 0)
            {
                ModelState.AddModelError("",
                    "College 'From' letter must come before 'To' letter alphabetically.");
                await PopulateRangeViewModel(model);
                return View(model);
            }

            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.Id == model.SelectedUserId);

            if (user == null)
            {
                ModelState.AddModelError("", "Selected user not found.");
                await PopulateRangeViewModel(model);
                return View(model);
            }

            // Get all colleges in range
            var collegesInRange = await _context.AffiliationCollegeMasters
                .Where(c => c.FacultyCode == model.SelectedFacultyId.ToString()
                         && c.Status == true
                         && c.CollegeName != null
                         && string.Compare(c.CollegeName.Substring(0, 1), model.CollegeFrom) >= 0
                         && string.Compare(c.CollegeName.Substring(0, 1), model.CollegeTo) <= 0)
                .ToListAsync();

            if (!collegesInRange.Any())
            {
                ModelState.AddModelError("",
                    "No colleges found in the specified range for this faculty.");
                await PopulateRangeViewModel(model);
                return View(model);
            }

            // Check for existing mapping
            var existing = await _context.TblCollegeMappings
                .FirstOrDefaultAsync(m => m.UserId == user.UserId
                                       && m.FacultyCode == model.SelectedFacultyId);

            if (existing != null)
            {
                ModelState.AddModelError("",
                    "This user already has a college mapping for this faculty.");
                await PopulateRangeViewModel(model);
                return View(model);
            }

            var mapping = new TblCollegeMapping
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FacultyCode = model.SelectedFacultyId,
                FromLetter = model.CollegeFrom,
                ToLetter = model.CollegeTo,

                CollegeFrom = collegesInRange
                    .OrderBy(c => c.CollegeCode)
                    .First().CollegeCode,

                CollegeTo = collegesInRange
                    .OrderBy(c => c.CollegeCode)
                    .Last().CollegeCode,

                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "System",
                IsActive = true
            };

            _context.TblCollegeMappings.Add(mapping);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Admin {Admin} assigned range {From}-{To} to user {UserName} ({Count} colleges)",
                User.Identity?.Name, model.CollegeFrom, model.CollegeTo,
                user.UserName, collegesInRange.Count);

            TempData["SuccessMessage"] =
                $"Successfully assigned {collegesInRange.Count} colleges ({model.CollegeFrom}-{model.CollegeTo}) to {user.UserName}.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /CollegeMapping/Edit/5
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var mapping = await _context.TblCollegeMappings
                .Include(m => m.FacultyCodeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mapping == null)
                return NotFound();

            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.UserId == mapping.UserId);

            ViewBag.User = user;
            ViewBag.Faculty = mapping.FacultyCodeNavigation;
            ViewBag.Faculties = await _context.Faculties
                .OrderBy(f => f.FacultyName)
                .ToListAsync();

            return View(mapping);
        }

        // POST: /CollegeMapping/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Edit(int id, TblCollegeMapping mapping)
        {
            if (id != mapping.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.User = await _context.TblRguhsFacultyUsers
                    .FirstOrDefaultAsync(u => u.UserId == mapping.UserId);

                ViewBag.Faculty = await _context.Faculties
                    .FirstOrDefaultAsync(f => f.FacultyId == mapping.FacultyCode);

                ViewBag.Faculties = await _context.Faculties
                    .OrderBy(f => f.FacultyName)
                    .ToListAsync();

                return View(mapping);
            }

            try
            {
                var existing = await _context.TblCollegeMappings.FindAsync(id);

                if (existing == null)
                    return NotFound();

                existing.FacultyCode = mapping.FacultyCode;

                existing.FromLetter = mapping.FromLetter.ToUpper().Trim();
                existing.ToLetter = mapping.ToLetter.ToUpper().Trim();

                existing.CollegeFrom = mapping.CollegeFrom.ToUpper().Trim();
                existing.CollegeTo = mapping.CollegeTo.ToUpper().Trim();

                existing.IsActive = mapping.IsActive;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "College mapping updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MappingExists(mapping.Id))
                    return NotFound();

                throw;
            }
        }
        // POST: /CollegeMapping/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Delete(int id)
        {
            var mapping = await _context.TblCollegeMappings.FindAsync(id);
            if (mapping != null)
            {
                _context.TblCollegeMappings.Remove(mapping);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "College mapping deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /CollegeMapping/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
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
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
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
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
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
            var combinedFrom = mappings.Min(m => m.CollegeFrom);
            var combinedTo = mappings.Max(m => m.CollegeTo);

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

        // ==================== HELPER METHODS ====================

        private bool MappingExists(int id)
        {
            return _context.TblCollegeMappings.Any(m => m.Id == id);
        }

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

        private async Task PopulateCreateViewModel(CollegeMappingCreateViewModel model)
        {
            model.AvailableUsers = await GetNonAdminUsersAsync(model.SelectedFacultyId);
            model.AvailableFaculties = await _context.Faculties
                .OrderBy(f => f.FacultyName)
                .ToListAsync();

            if (model.SelectedFacultyId > 0)
            {
                var query = _context.AffiliationCollegeMasters
                    .Where(c => c.FacultyCode == model.SelectedFacultyId.ToString()
                             && c.Status == true);

                // Apply alphabetical range filter
                if (!string.IsNullOrWhiteSpace(model.FromLetter))
                {
                    query = query.Where(c => c.CollegeName != null
                        && string.Compare(c.CollegeName.Substring(0, 1).ToUpper(), model.FromLetter.ToUpper()) >= 0);
                }
                if (!string.IsNullOrWhiteSpace(model.ToLetter))
                {
                    query = query.Where(c => c.CollegeName != null
                        && string.Compare(c.CollegeName.Substring(0, 1).ToUpper(), model.ToLetter.ToUpper()) <= 0);
                }

                model.AvailableColleges = await query
                    .OrderBy(c => c.CollegeName)
                    .ToListAsync();
            }
        }

        // GET: /CollegeMapping/GetCollegesByFaculty?facultyId=X&collegeFrom=A&collegeTo=Z
        [HttpGet]
        public async Task<IActionResult> GetCollegesByFaculty(int facultyId, string? fromLetter, string? toLetter)
        {
            var colleges = await _context.AffiliationCollegeMasters
                .Where(c => c.FacultyCode == facultyId.ToString())
                .OrderBy(c => c.CollegeName)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(fromLetter))
            {
                colleges = colleges
                    .Where(c => !string.IsNullOrWhiteSpace(c.CollegeName) &&
                                string.Compare(
                                    c.CollegeName.Substring(0, 1),
                                    fromLetter,
                                    StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(toLetter))
            {
                colleges = colleges
                    .Where(c => !string.IsNullOrWhiteSpace(c.CollegeName) &&
                                string.Compare(
                                    c.CollegeName.Substring(0, 1),
                                    toLetter,
                                    StringComparison.OrdinalIgnoreCase) <= 0)
                    .ToList();
            }

            return Json(colleges.Select(c => new
            {
                code = c.CollegeCode,
                name = c.CollegeName ?? "",
                town = c.CollegeTown ?? ""
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
