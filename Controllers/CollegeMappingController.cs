using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using System.Security.Claims;

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
                    UserDesignation = users.FirstOrDefault(u => u.UserId == g.Key.UserId && u.Faculty == g.Key.FacultyCode)?.DesignationDescription,
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

            // Get selected user first
            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.UserId == model.SelectedUserId && u.Faculty == model.SelectedFacultyId);

            if (user == null)
            {
                ModelState.AddModelError("", "Selected user not found.");
                await PopulateCreateViewModel(model);
                return View(model);
            }

            // ---------------------------------------------------------
            // 1. Prevent duplicate mapping for the SAME user + faculty
            // ---------------------------------------------------------
            var existingUserMapping = await _context.TblCollegeMappings
                .FirstOrDefaultAsync(m =>
                    m.UserId == user.UserId &&
                    m.FacultyCode == model.SelectedFacultyId &&
                    m.IsActive);

            if (existingUserMapping != null)
            {
                ModelState.AddModelError("",
                    "This user already has an active college mapping for this faculty. " +
                    "Please edit the existing mapping instead.");

                await PopulateCreateViewModel(model);
                return View(model);
            }

            // ---------------------------------------------------------
            // 2. Get selected colleges
            // ---------------------------------------------------------
            var selectedColleges = await _context.AffiliationCollegeMasters
                .Where(c => 
                    c.FacultyCode == model.SelectedFacultyId.ToString() &&  
                    model.SelectedCollegeCodes.Contains(c.CollegeCode))
                .OrderBy(c => c.CollegeName)
                .ToListAsync();

            if (!selectedColleges.Any())
            {
                ModelState.AddModelError("", "No valid colleges selected.");
                await PopulateCreateViewModel(model);
                return View(model);
            }

            // ---------------------------------------------------------
            // 3. Check overlap ONLY if user is NOT an admin
            // ---------------------------------------------------------
            if (user.IsAdmin != true)
            {
                var selectedCollegeCodes = selectedColleges
                    .Select(c => c.CollegeCode)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                // Get existing active mappings for this faculty
                var existingMappings = await _context.TblCollegeMappings
                    .Where(m =>
                        m.FacultyCode == model.SelectedFacultyId &&
                        m.IsActive &&
                        m.UserId != user.UserId)
                    .ToListAsync();

                

                // --------------------------------------------------------- 
                // Get the users associated with those mappings. 
                // We need this to identify Admin mappings. 
                // ---------------------------------------------------------

                var existingUserIds = existingMappings
                    .Select(m => m.UserId)
                    .Distinct()
                    .ToList();

                var adminUserIds = await _context.TblRguhsFacultyUsers
                    .Where(u =>
                        u.Faculty == model.SelectedFacultyId &&
                        u.IsAdmin == true &&
                        existingUserIds.Contains(u.UserId))
                    .Select(u => u.UserId)
                    .Distinct()
                    .ToListAsync();


                // --------------------------------------------------------
                // IMPORTANT:
                // Remove Mappings belonging to Admin users.
                //
                // Therefore, if an admin has all colleges assigned, 
                // those colleges will NOT block a normal user

                existingMappings = existingMappings
                    .Where(m => !adminUserIds.Contains(m.UserId))
                    .ToList();

                // --------------------------------------------------------- 
                // Get colleges ONLY for the selected faculty. 
                // ---------------------------------------------------------
                var facultyColleges = await _context.AffiliationCollegeMasters 
                    .Where(c => 
                        c.FacultyCode == model.SelectedFacultyId.ToString() && 
                        c.CollegeName != null && 
                        c.CollegeCode != null) 
                    .ToListAsync();

                var overlappingMappings = new List<TblCollegeMapping>();

                foreach (var existingMapping in existingMappings)
                {
                    // Get colleges covered by existing mapping
                    var existingColleges = facultyColleges
                        .Where(c =>
                        {
                            var collegeName = c.CollegeName?.Trim();

                            if (string.IsNullOrEmpty(collegeName))
                                return false;

                            var firstLetter = collegeName.Substring(0, 1);

                            return string.Compare(
                                       firstLetter,
                                       existingMapping.FromLetter,
                                       StringComparison.OrdinalIgnoreCase) >= 0
                                   &&
                                   string.Compare(
                                       firstLetter,
                                       existingMapping.ToLetter,
                                       StringComparison.OrdinalIgnoreCase) <= 0;
                        })
                        .Where(c =>
                            string.Compare(
                                c.CollegeCode,
                                existingMapping.CollegeFrom,
                                StringComparison.OrdinalIgnoreCase) >= 0
                            &&
                            string.Compare(
                                c.CollegeCode,
                                existingMapping.CollegeTo,
                                StringComparison.OrdinalIgnoreCase) <= 0)
                        .ToList();

                    if (existingColleges.Any(c =>
                            selectedCollegeCodes.Contains(c.CollegeCode)))
                    {
                        overlappingMappings.Add(existingMapping);
                    }
                }

                if (overlappingMappings.Any())
                {
                    var users = string.Join(
                        ", ",
                        overlappingMappings
                            .Select(x => x.UserName)
                            .Distinct());

                    ModelState.AddModelError(
                        "",
                        $"One or more selected colleges are already assigned to: {users}. " +
                        "Only Admin users can have overlapping college assignments.");

                    await PopulateCreateViewModel(model);
                    return View(model);
                }
            }

            // ---------------------------------------------------------
            // 4. Determine alphabetical range
            // ---------------------------------------------------------
            var firstCollege = selectedColleges.First();
            var lastCollege = selectedColleges.Last();

            var firstCollegeName = firstCollege.CollegeName?.Trim();
            var lastCollegeName = lastCollege.CollegeName?.Trim();

            var firstLetter = !string.IsNullOrEmpty(firstCollegeName)
                ? firstCollegeName.Substring(0, 1).ToUpper()
                : "A";

            var lastLetter = !string.IsNullOrEmpty(lastCollegeName)
                ? lastCollegeName.Substring(0, 1).ToUpper()
                : "Z";

            // ---------------------------------------------------------
            // 5. Create mapping
            // ---------------------------------------------------------
            var mapping = new TblCollegeMapping
            {
                UserId = user.UserId,
                UserName = user.UserName ?? "",
                FacultyCode = model.SelectedFacultyId,

                FromLetter = firstLetter,
                ToLetter = lastLetter,

                CollegeFrom = firstCollege.CollegeCode,
                CollegeTo = lastCollege.CollegeCode,

                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "System",
                IsActive = true
            };

            _context.TblCollegeMappings.Add(mapping);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User {UserId} ({UserName}) assigned range {From}-{To} ({Count} colleges), IsAdmin={IsAdmin}",
                user.UserId,
                user.UserName,
                firstLetter,
                lastLetter,
                selectedColleges.Count,
                user.IsAdmin);

            TempData["SuccessMessage"] =
                $"Successfully assigned {selectedColleges.Count} college(s) " +
                $"({firstLetter}-{lastLetter}) to {user.UserName}.";

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
                .FirstOrDefaultAsync(u => 
                    u.Id == model.SelectedUserId && 
                    u.Faculty == model.SelectedFacultyId);

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

        // GET: /CollegeMapping/MyMappings
        // For regular users to view their own college mappings
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyMappings()
        {
            // Get current user's ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim.Value);

            // Get the user details
            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            // Get all mappings for this user
            var mappings = await _context.TblCollegeMappings
                .Include(m => m.FacultyCodeNavigation)
                .Where(m => m.UserId == user.UserId && m.IsActive == true)
                .ToListAsync();

            if (!mappings.Any())
            {
                ViewBag.Message = "No college mappings assigned to you.";
                return View(new List<CollegeMappingWithCollegesViewModel>());
            }

            // For each mapping, get the colleges in the assigned range
            var result = new List<CollegeMappingWithCollegesViewModel>();

            foreach (var mapping in mappings)
            {
                var faculty = await _context.Faculties
                    .FirstOrDefaultAsync(f => f.FacultyId == mapping.FacultyCode);

                // Get colleges in the assigned range (using CollegeFrom and CollegeTo as code range)
                var colleges = await _context.AffiliationCollegeMasters
                    .Where(c => c.FacultyCode == mapping.FacultyCode.ToString()
                             && c.Status == true
                             && c.CollegeCode != null)
                    .OrderBy(c => c.CollegeCode)
                    .ToListAsync();

                // Filter colleges within the code range
                var fromCode = mapping.CollegeFrom?.ToUpper().Trim() ?? "";
                var toCode = mapping.CollegeTo?.ToUpper().Trim() ?? "";

                var collegesInRange = colleges
                    .Where(c =>
                    {
                        var code = c.CollegeCode?.ToUpper().Trim() ?? "";
                        return string.Compare(code, fromCode, StringComparison.OrdinalIgnoreCase) >= 0
                            && string.Compare(code, toCode, StringComparison.OrdinalIgnoreCase) <= 0;
                    })
                    .ToList();

                result.Add(new CollegeMappingWithCollegesViewModel
                {
                    Mapping = mapping,
                    FacultyName = faculty?.FacultyName ?? "Unknown Faculty",
                    UserDesignation = user.DesignationDescription ?? "",
                    Colleges = collegesInRange,
                    CollegeCount = collegesInRange.Count,
                    FromLetter = mapping.FromLetter,
                    ToLetter = mapping.ToLetter,
                    CollegeFromCode = mapping.CollegeFrom,
                    CollegeToCode = mapping.CollegeTo
                });
            }

            ViewBag.UserName = user.UserName;
            ViewBag.UserDesignation = user.DesignationDescription;
            ViewBag.FacultyId = user.Faculty;

            return View(result);
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

            var fromCollege = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == mapping.CollegeFrom);

            var toCollege = await _context.AffiliationCollegeMasters
                .FirstOrDefaultAsync(c => c.CollegeCode == mapping.CollegeTo);

            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.UserId == mapping.UserId);

            var faculties = await _context.Faculties
                .OrderBy(f => f.FacultyName)
                .ToListAsync();

            // Load colleges for the selected letter range
            var colleges = await _context.AffiliationCollegeMasters
                .Where(c => c.FacultyCode == mapping.FacultyCode.ToString()
                         && c.CollegeName != null)
                .OrderBy(c => c.CollegeName)
                .ToListAsync();

            colleges = colleges
                .Where(c =>
                {
                    var letter = c.CollegeName!.Substring(0, 1).ToUpper();

                    return string.Compare(letter, mapping.FromLetter, StringComparison.OrdinalIgnoreCase) >= 0
                        && string.Compare(letter, mapping.ToLetter, StringComparison.OrdinalIgnoreCase) <= 0;
                })
                .ToList();

            var model = new CollegeMappingEditViewModel
            {
                Id = mapping.Id,
                FacultyCode = mapping.FacultyCode,
                FromLetter = mapping.FromLetter?.ToUpper() ?? "A",
                ToLetter = mapping.ToLetter?.ToUpper() ?? "Z",
                CollegeFrom = mapping.CollegeFrom?.ToUpper() ?? "",
                CollegeTo = mapping.CollegeTo?.ToUpper() ?? "",
                IsActive = mapping.IsActive,
                UserName = mapping.UserName,
                UserId = mapping.UserId.ToString(),
                UserDesignation = user?.DesignationDescription ?? "",
                FacultyName = mapping.FacultyCodeNavigation?.FacultyName ?? "",
                CreatedDate = mapping.CreatedDate ?? DateTime.MinValue,
                CreatedBy = mapping.CreatedBy ?? "",
                AvailableFaculties = faculties,
                AvailableColleges = colleges.Select(c => new SelectCollegeOption
                {
                    Code = c.CollegeCode,
                    Name = c.CollegeName ?? ""
                }).ToList(),
                SelectedCollegeFromCode = mapping.CollegeFrom?.ToUpper() ?? "",
                SelectedCollegeToCode = mapping.CollegeTo?.ToUpper() ?? ""
            };

            return View(model);
        }

        // POST: /CollegeMapping/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
        public async Task<IActionResult> Edit(int id, CollegeMappingEditViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // Reload dropdown data
                var faculties = await _context.Faculties
                    .OrderBy(f => f.FacultyName)
                    .ToListAsync();

                var colleges = await _context.AffiliationCollegeMasters
                    .Where(c => c.FacultyCode == model.FacultyCode.ToString()
                             && c.Status == true
                             && c.CollegeName != null)
                    .OrderBy(c => c.CollegeName)
                    .ToListAsync();

                colleges = colleges
                    .Where(c =>
                    {
                        var letter = c.CollegeName!.Substring(0, 1).ToUpper();

                        return string.Compare(letter, model.FromLetter, StringComparison.OrdinalIgnoreCase) >= 0
                            && string.Compare(letter, model.ToLetter, StringComparison.OrdinalIgnoreCase) <= 0;
                    })
                    .ToList();

                model.AvailableFaculties = faculties;
                model.AvailableColleges = colleges.Select(c => new SelectCollegeOption
                {
                    Code = c.CollegeCode,
                    Name = c.CollegeName ?? ""
                }).ToList();

                return View(model);
            }

            try
            {
                var existing = await _context.TblCollegeMappings.FindAsync(id);

                if (existing == null)
                    return NotFound();

                // Check for overlapping mappings with OTHER users for the same faculty
                var overlapMappings = await _context.TblCollegeMappings
                    .Where(m => m.Id != id
                             && m.FacultyCode == model.FacultyCode
                             && m.IsActive == true)
                    .Include(m => m.FacultyCodeNavigation)
                    .ToListAsync();

                // Get college names for the new range to compare alphabetically
                var newFromCollege = await _context.AffiliationCollegeMasters
                    .FirstOrDefaultAsync(c => c.CollegeCode == model.CollegeFrom.ToUpper().Trim());
                var newToCollege = await _context.AffiliationCollegeMasters
                    .FirstOrDefaultAsync(c => c.CollegeCode == model.CollegeTo.ToUpper().Trim());

                if (newFromCollege != null && newToCollege != null)
                {
                    var newFromLetter = newFromCollege.CollegeName?.Substring(0, 1).ToUpper() ?? "";
                    var newToLetter = newToCollege.CollegeName?.Substring(0, 1).ToUpper() ?? "";

                    var conflicts = new List<string>();

                    foreach (var otherMapping in overlapMappings)
                    {
                        // Get the college names for the existing mapping's range
                        var otherFromCollege = await _context.AffiliationCollegeMasters
                            .FirstOrDefaultAsync(c => c.CollegeCode == otherMapping.CollegeFrom);
                        var otherToCollege = await _context.AffiliationCollegeMasters
                            .FirstOrDefaultAsync(c => c.CollegeCode == otherMapping.CollegeTo);

                        if (otherFromCollege != null && otherToCollege != null)
                        {
                            var otherFromLetter = otherFromCollege.CollegeName?.Substring(0, 1).ToUpper() ?? "";
                            var otherToLetter = otherToCollege.CollegeName?.Substring(0, 1).ToUpper() ?? "";

                            // Check if ranges overlap alphabetically
                            // Two ranges [A, B] and [C, D] overlap if: A <= D && C <= B
                            bool rangesOverlap = string.Compare(newFromLetter, otherToLetter, StringComparison.OrdinalIgnoreCase) <= 0
                                              && string.Compare(otherFromLetter, newToLetter, StringComparison.OrdinalIgnoreCase) <= 0;

                            if (rangesOverlap)
                            {
                                var user = await _context.TblRguhsFacultyUsers
                                    .FirstOrDefaultAsync(u => u.UserId == otherMapping.UserId);
                                var facultyName = otherMapping.FacultyCodeNavigation?.FacultyName ?? "Unknown Faculty";
                                conflicts.Add($"User '{user?.UserName ?? otherMapping.UserName}' ({user?.DesignationDescription ?? "Unknown"}) in {facultyName} has range {otherFromLetter}-{otherToLetter} ({otherMapping.CollegeFrom}-{otherMapping.CollegeTo})");
                            }
                        }
                    }

                    if (conflicts.Any())
                    {
                        ModelState.AddModelError("",
                            $"⚠️ <strong>Overlap Detected!</strong> The college range <strong>{newFromLetter}-{newToLetter}</strong> ({model.CollegeFrom}-{model.CollegeTo}) overlaps with existing mappings:<br/>" +
                            string.Join("<br/>", conflicts.Select((c, i) => $"{i + 1}. {c}")) +
                            "<br/>Please adjust the From/To letters or college codes to avoid conflicts.");
                        await PopulateEditViewModel(model);
                        return View(model);
                    }
                }

                existing.FacultyCode = model.FacultyCode;

                existing.FromLetter = model.FromLetter.ToUpper().Trim();
                existing.ToLetter = model.ToLetter.ToUpper().Trim();

                existing.CollegeFrom = model.CollegeFrom.ToUpper().Trim();
                existing.CollegeTo = model.CollegeTo.ToUpper().Trim();

                existing.IsActive = model.IsActive;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "College mapping updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MappingExists(model.Id))
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

        private async Task PopulateEditViewModel(CollegeMappingEditViewModel model)
        {
            var faculties = await _context.Faculties
                .OrderBy(f => f.FacultyName)
                .ToListAsync();

            var colleges = await _context.AffiliationCollegeMasters
                .Where(c => c.FacultyCode == model.FacultyCode.ToString()
                         && c.Status == true
                         && c.CollegeName != null)
                .OrderBy(c => c.CollegeName)
                .ToListAsync();

            colleges = colleges
                .Where(c =>
                {
                    var letter = c.CollegeName!.Substring(0, 1).ToUpper();

                    return string.Compare(letter, model.FromLetter, StringComparison.OrdinalIgnoreCase) >= 0
                        && string.Compare(letter, model.ToLetter, StringComparison.OrdinalIgnoreCase) <= 0;
                })
                .ToList();

            model.AvailableFaculties = faculties;
            model.AvailableColleges = colleges.Select(c => new SelectCollegeOption
            {
                Code = c.CollegeCode,
                Name = c.CollegeName ?? ""
            }).ToList();
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
