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
                .Include(m => m.AllCollegesMapping)
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

            // Get all records belonging to selective mapping groups
            var selectedIds = mappings
                .Where(m => m.AllCollegesMapping?.SelectedId != null)
                .Select(m => m.AllCollegesMapping!.SelectedId!.Value)
                .Distinct()
                .ToList();

            var selectiveCollegeRecords = await _context.TblAllCollegesMappings
                .Where(x =>
                    x.SelectedId.HasValue &&
                    selectedIds.Contains(x.SelectedId.Value))
                .ToListAsync();

            // Get user details
            var userIds = mappings.Select(m => m.UserId).Distinct().ToList();
            var users = await _context.TblRguhsFacultyUsers
                .Where(u => userIds.Contains(u.UserId))
                .ToListAsync();

            // Group by UserId + FacultyCode to show one row per user per faculty
            var groupedMappings = mappings
                .GroupBy(m => new { m.UserId, m.FacultyCode })
                .Select(g =>
                {
                    var mappingRecords = g
                        .Select(m => m.AllCollegesMapping)
                        .Where(c => c != null)
                        .ToList();

                    // Separate serial and selective mappings
                    var serialRecords = mappingRecords
                        .Where(c => c!.SelectedId == null)
                        .ToList();

                    var selectiveGroupIds = mappingRecords
                        .Where(c => c!.SelectedId.HasValue)
                        .Select(c => c!.SelectedId!.Value)
                        .Distinct()
                        .ToList();

                    // Get all individual college records for these selective groups
                    var selectiveRecords = selectiveCollegeRecords
                        .Where(c => c.SelectedId.HasValue &&
                                    selectiveGroupIds.Contains(c.SelectedId.Value))
                        .ToList();

                    // Combine serial and selective records for display
                    var allRecords = serialRecords
                        .Concat(selectiveRecords)
                        .ToList();

                    var fromLetters = allRecords
                        .Select(c => c.FromLetter)
                        .OrderBy(v => v)
                        .ToList();

                    var toLetters = allRecords
                        .Select(c => c.ToLetter)
                        .OrderBy(v => v)
                        .ToList();

                    var fromCodes = allRecords
                        .Select(c => c.FromCollegeCode)
                        .OrderBy(v => v)
                        .ToList();

                    var toCodes = allRecords
                        .Select(c => c.ToCollegeCode)
                        .OrderBy(v => v)
                        .ToList();

                    return new
                    {
                        UserId = g.Key.UserId,
                        FacultyCode = g.Key.FacultyCode,
                        UserName = g.First().UserName,
                        MappingType = selectiveGroupIds.Any() ? "Selective" : "Serial",
                        SelectedCollegeCodes = allRecords.Select(e => e.FromCollegeCode).Distinct().OrderBy(e => e).ToList(),

                        FromLetter = fromLetters.FirstOrDefault() ?? "",
                        ToLetter = toLetters.LastOrDefault() ?? "",

                        CollegeFrom = fromCodes.FirstOrDefault() ?? "",
                        CollegeTo = toCodes.LastOrDefault() ?? "",

                        IsActive = g.All(m => m.IsActive),
                        CreatedDate = g.Min(m => m.CreatedDate),
                        CreatedBy = g.First().CreatedBy,

                        FacultyName = g.First().FacultyCodeNavigation?.FacultyName,

                        UserDesignation = users
                            .FirstOrDefault(u =>
                                u.UserId == g.Key.UserId &&
                                u.Faculty == g.Key.FacultyCode)
                            ?.DesignationDescription,

                        

                        MappingCount = g.Count(),
                        MappingIds = g.Select(m => m.Id).ToList(),
                        VerificationLevel = g.First().VerificationLevel
                    };
                })
                .ToList();

            var mappingList = groupedMappings.Select(g => new CollegeMappingWithUser
            {
                Mapping = new TblCollegeMapping
                {
                    Id = g.MappingIds.First(),
                    UserId = g.UserId,
                    UserName = g.UserName,
                    FacultyCode = g.FacultyCode,

                    // New relationship
                    AllCollegesMapping = new TblAllCollegesMapping
                    {
                        FromLetter = g.FromLetter,
                        ToLetter = g.ToLetter,
                        FromCollegeCode = g.CollegeFrom,
                        ToCollegeCode = g.CollegeTo
                    },

                    IsActive = g.IsActive,
                    CreatedDate = g.CreatedDate,
                    CreatedBy = g.CreatedBy,
                    VerificationLevel = g.VerificationLevel
                },

                UserDesignation = g.UserDesignation,
                FacultyName = g.FacultyName,
                MappingType = g.MappingType,
                SelectedCollegeCodes = g.SelectedCollegeCodes
            }).ToList();

            var faculties = await _context.Faculties.Where(e => e.Status == "Active").OrderBy(f => f.FacultyName).ToListAsync();

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
                    .Where(e => e.Status == "Active")
                    .OrderBy(f => f.FacultyName)
                    .ToListAsync()
            };

            if (facultyId.HasValue && facultyId.Value > 0)
            {
                model.SelectedFacultyId = facultyId.Value;
                model.AvailableColleges = await _context.AffiliationCollegeMasters
                    .Where(c => c.FacultyCode == facultyId.Value.ToString() && c.Status == true)
                    .OrderBy(c => c.CollegeCode)
                    .ThenBy(c => c.CollegeName)
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

            // 3. Get selected colleges
            var selectedCollegeCodes = model.SelectedCollegeCodes
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var selectedColleges = await _context.AffiliationCollegeMasters
                .Where(c =>
                    c.FacultyCode == model.SelectedFacultyId.ToString() &&
                    c.Status == true &&
                    selectedCollegeCodes.Contains(c.CollegeCode))
                .OrderBy(c => c.CollegeName)
                .ToListAsync();

            if (!selectedColleges.Any())
            {
                ModelState.AddModelError("", "No valid colleges selected.");
                await PopulateCreateViewModel(model);
                return View(model);
            }

            // Ensure all submitted college codes are valid
            if (selectedColleges.Count != selectedCollegeCodes.Count)
            {
                ModelState.AddModelError("", "One or more selected colleges are invalid.");
                await PopulateCreateViewModel(model);
                return View(model);
            }

            // CHECKING SERIAL 
            var orderedCollegeCodes = await _context.AffiliationCollegeMasters
                .Where(e =>
                    e.FacultyCode == model.SelectedFacultyId.ToString() &&
                    e.Status == true &&
                    e.CollegeCode != null
                )
                .OrderBy(e => e.CollegeCode)
                .Select(e => e.CollegeCode)
                .ToListAsync();

            var collegePositions = orderedCollegeCodes
                .Select((code, index) => new { code, index })
                .ToDictionary(
                e => e.code,
                e => e.index,
                StringComparer.OrdinalIgnoreCase);

            var selectedIndexes = selectedCollegeCodes
                .Select(code => collegePositions[code])
                .OrderBy(index => index)
                .ToList();

            bool isSerial = selectedIndexes.Last() - selectedIndexes.First() + 1 == selectedIndexes.Count;

            // Arranage selected colleges in order
            var selectedCollegesByCode = selectedColleges
                .ToDictionary(e => e.CollegeCode, StringComparer.OrdinalIgnoreCase);

            var orderedSelectedColleges = selectedIndexes
                .Select(index =>
                    selectedCollegesByCode[orderedCollegeCodes[index]])
                .ToList();


            // ---------------------------------------------------------
            // 3. Check overlap ONLY if user is NOT an admin
            // ---------------------------------------------------------
            if (user.IsAdmin != true)
            {
                var selectedCodes = selectedColleges
                    .Select(c => c.CollegeCode)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                // Get existing active mappings for this faculty
                var existingMappings = await _context.TblCollegeMappings
                    .Include(m=> m.AllCollegesMapping)
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
                    var range = existingMapping.AllCollegesMapping;

                    if (range == null) continue;

                    // Get colleges covered by existing mapping
                    var existingColleges = facultyColleges
                        .Where(c =>
                        {
                            var collegeName = c.CollegeName?.Trim();

                            if (string.IsNullOrEmpty(collegeName))
                                return false;

                            var firstLetter = collegeName.Substring(0, 1);

                            return string.Compare(firstLetter, range.FromLetter, StringComparison.OrdinalIgnoreCase) >= 0
                                   &&
                                   string.Compare(firstLetter, range.ToLetter, StringComparison.OrdinalIgnoreCase) <= 0;
                        })
                        .Where(c =>
                            string.Compare(c.CollegeCode, range.FromCollegeCode, StringComparison.OrdinalIgnoreCase) >= 0
                            &&
                            string.Compare(c.CollegeCode, range.ToCollegeCode, StringComparison.OrdinalIgnoreCase) <= 0)
                        .ToList();

                    if (existingColleges.Any(c => selectedCollegeCodes.Contains(c.CollegeCode)))
                    {
                        overlappingMappings.Add(existingMapping);
                    }
                }

                // if (overlappingMappings.Any())
                // {
                //     var users = string.Join(
                //         ", ",
                //         overlappingMappings
                //             .Select(x => x.UserName)
                //             .Distinct());

                //     ModelState.AddModelError(
                //         "",
                //         $"One or more selected colleges are already assigned to: {users}. " +
                //         "Only Admin users can have overlapping college assignments.");

                //     await PopulateCreateViewModel(model);
                //     return View(model);
                // }
            }

            // ---------------------------------------------------------
            // 4. Determine alphabetical range
            // ---------------------------------------------------------

            // ---------------------------------------------------------
            // 4. CREATE ALL COLLEGES MAPPING
            // ---------------------------------------------------------

            TblAllCollegesMapping allCollegesMapping;

            if (isSerial)
            {
                // SERIAL COLLEGES
                // Save only one record with the range.

                var firstCollege = orderedSelectedColleges.First();
                var lastCollege = orderedSelectedColleges.Last();

                var firstLetter = firstCollege.CollegeName!.Trim()
                    .Substring(0, 1).ToUpper();

                var lastLetter = lastCollege.CollegeName!.Trim()
                    .Substring(0, 1).ToUpper();

                allCollegesMapping = new TblAllCollegesMapping
                {
                    SelectedId = null,

                    FromLetter = firstLetter,
                    ToLetter = lastLetter,

                    FromCollegeCode = firstCollege.CollegeCode,
                    ToCollegeCode = lastCollege.CollegeCode,

                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Identity?.Name ?? "System"
                };

                _context.TblAllCollegesMappings.Add(allCollegesMapping);
            }
            else
            {
                // NON-SERIAL COLLEGES
                // Save each college separately with the same SelectedId.

                var maxSelectedId = await _context.TblAllCollegesMappings
                    .Where(x => x.SelectedId.HasValue)
                    .MaxAsync(x => (int?)x.SelectedId) ?? 0;

                var selectedId = maxSelectedId + 1;

                var selectedMappingRecords = new List<TblAllCollegesMapping>();

                foreach (var college in orderedSelectedColleges)
                {
                    var letter = college.CollegeName!.Trim()
                        .Substring(0, 1).ToUpper();

                    var selectedMapping = new TblAllCollegesMapping
                    {
                        SelectedId = selectedId,

                        FromLetter = letter,
                        ToLetter = letter,

                        FromCollegeCode = college.CollegeCode,
                        ToCollegeCode = college.CollegeCode,

                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity?.Name ?? "System"
                    };

                    selectedMappingRecords.Add(selectedMapping);
                }

                _context.TblAllCollegesMappings.AddRange(selectedMappingRecords);

                // Use the first record as the parent reference.
                allCollegesMapping = selectedMappingRecords.First();
            }

            // ---------------------------------------------------------
            // 5. CREATE COLLEGE MAPPING
            // ---------------------------------------------------------

            var maxLevel = await _context.TblCollegeMappings
                .Where(m => m.FacultyCode == model.SelectedFacultyId)
                .MaxAsync(m => (int?)m.VerificationLevel) ?? 0;

            var mapping = new TblCollegeMapping
            {
                UserId = user.UserId,
                UserName = user.UserName ?? "",
                FacultyCode = model.SelectedFacultyId,

                // Automatically establishes AllCollegesMappingId
                AllCollegesMapping = allCollegesMapping,

                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "System",
                IsActive = true,

                VerificationLevel = model.NextVerificationLevel.HasValue
                    ? model.NextVerificationLevel.Value
                    : (model.VerificationLevel.HasValue && model.VerificationLevel > 0
                        ? model.VerificationLevel.Value
                        : maxLevel + 1)
            };

            _context.TblCollegeMappings.Add(mapping);


            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User {UserId} ({UserName}) assigned {Count} colleges. Serial={IsSerial}, IsAdmin={IsAdmin}",
                user.UserId,
                user.UserName,
                selectedColleges.Count,
                isSerial,
                user.IsAdmin);

            TempData["SuccessMessage"] =
                $"Successfully assigned {selectedColleges.Count} college(s) to {user.UserName}.";

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

            var allCollegesMapping = new TblAllCollegesMapping
            {
                FromLetter = model.CollegeFrom,
                ToLetter = model.CollegeTo,

                FromCollegeCode = collegesInRange.First().CollegeCode,

                ToCollegeCode = collegesInRange.Last().CollegeCode,

                IsActive = true,
                CreatedDate = DateTime.Now,
                CreatedBy = User.Identity?.Name ?? "System"
            };

            _context.TblAllCollegesMappings.Add(allCollegesMapping);

            var mapping = new TblCollegeMapping
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FacultyCode = model.SelectedFacultyId,
                
                AllCollegesMapping = allCollegesMapping,

                //CollegeFrom = collegesInRange
                //    .OrderBy(c => c.CollegeCode)
                //    .First().CollegeCode,

                //CollegeTo = collegesInRange
                //    .OrderBy(c => c.CollegeCode)
                //    .Last().CollegeCode,

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

            var courseLevel = HttpContext.Session.GetString("CourseLevel");

            int userId = int.Parse(userIdClaim.Value);

            // Get the user details
            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            // Get all mappings for this user
            var mappings = await _context.TblCollegeMappings
                .Include(m => m.FacultyCodeNavigation)
                .Include(m => m.AllCollegesMapping)
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
                var range = mapping.AllCollegesMapping;

                var facultyName = mapping.FacultyCodeNavigation?.FacultyName ?? "Unknown Faculty";

                // If no range configuration exists, there are no colleges to display
                if (range == null)
                {
                    result.Add(new CollegeMappingWithCollegesViewModel
                    {
                        Mapping = mapping,
                        FacultyName = facultyName,
                        UserDesignation = user.DesignationDescription ?? "",
                        Colleges = new List<CollegeDisplayViewModel>(),
                        CollegeCount = 0
                    });

                    continue;
                }

                // Get colleges in the assigned range (using CollegeFrom and CollegeTo as code range)
                var colleges = await _context.AffiliationCollegeMasters
                    .Where(c => c.FacultyCode == mapping.FacultyCode.ToString()
                             && c.Status == true
                             && c.CollegeCode != null)
                    .OrderBy(c => c.CollegeCode)
                    .ToListAsync();

                // Filter colleges within the code range
                var fromCode = range.FromCollegeCode?.ToUpper().Trim() ?? "";
                var toCode = range.ToCollegeCode?.ToUpper().Trim() ?? "";

                var collegesInRange = colleges
                    .Where(c =>
                    {
                        var code = c.CollegeCode?.ToUpper().Trim() ?? "";
                        return string.Compare(code, fromCode, StringComparison.OrdinalIgnoreCase) >= 0
                            && string.Compare(code, toCode, StringComparison.OrdinalIgnoreCase) <= 0;
                    })
                    .ToList();


                var collegeCodes = collegesInRange
                    .Select(c => c.CollegeCode)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList();

                var applicationNumbers = await _context.FinalDentalSubmissions
                    .AsNoTracking()
                    .Where(e => e.IsActive &&
                                e.CourseLevel == courseLevel &&
                                collegeCodes.Contains(e.CollegeCode))
                    .Select(e => new
                    {
                        e.CollegeCode,
                        e.ApplicationNumber
                    })
                    .ToListAsync();

                var applicationNumberByCollege = applicationNumbers
                    .GroupBy(e => e.CollegeCode)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Select(d => d.ApplicationNumber).FirstOrDefault()
                    );

                // Convert to display model
                var collegeDisplayList = collegesInRange
                    .Select(c => new CollegeDisplayViewModel
                    {
                        CollegeCode = c.CollegeCode,
                        CollegeName = c.CollegeName ?? "",
                        CollegeTown = c.CollegeTown,

                        ApplicationNumber = applicationNumberByCollege.GetValueOrDefault(c.CollegeCode)
                    })
                    .ToList();

                result.Add(new CollegeMappingWithCollegesViewModel
                {
                    Mapping = mapping,
                    FacultyName = facultyName,
                    UserDesignation = user.DesignationDescription ?? "",
                    Colleges = collegeDisplayList,
                    CollegeCount = collegesInRange.Count,
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
                .Include(m => m.AllCollegesMapping)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mapping == null)
                return NotFound();

            var allCollegesMapping = mapping.AllCollegesMapping;

            var user = await _context.TblRguhsFacultyUsers
                .FirstOrDefaultAsync(u =>
                    u.UserId == mapping.UserId &&
                    u.Faculty == mapping.FacultyCode);


            var faculties = await _context.Faculties
                .OrderBy(f => f.FacultyName)
                .ToListAsync();

            // Get the configured letter range
            var fromLetter = allCollegesMapping?.FromLetter?.ToUpper() ?? "A";
            var toLetter = allCollegesMapping?.ToLetter?.ToUpper() ?? "Z";

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

                    return string.Compare(letter, fromLetter, StringComparison.OrdinalIgnoreCase) >= 0
                        && string.Compare(letter, toLetter, StringComparison.OrdinalIgnoreCase) <= 0;
                })
                .ToList();

            var model = new CollegeMappingEditViewModel
            {
                Id = mapping.Id,
                FacultyCode = mapping.FacultyCode,
                FromLetter = fromLetter,
                ToLetter = toLetter,

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
                
                VerificationLevel = mapping.VerificationLevel
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
                var currentRange = existing.AllCollegesMapping;

                if (currentRange == null)
                {
                    // Create a new range configuration if one does not exist
                    currentRange = new TblAllCollegesMapping
                    {
                        FromLetter = model.FromLetter.ToUpper().Trim(),
                        ToLetter = model.ToLetter.ToUpper().Trim(),
                        FromCollegeCode = "",
                        ToCollegeCode = "",
                        IsActive = model.IsActive,
                        CreatedDate = DateTime.Now,
                        CreatedBy = User.Identity?.Name
                    };

                    _context.TblAllCollegesMappings.Add(currentRange);

                    existing.AllCollegesMapping = currentRange;
                }
                else
                {
                    // Check whether this range configuration is shared
                    var referenceCount = await _context.TblCollegeMappings
                        .CountAsync(m => m.AllCollegesMappingId == existing.AllCollegesMappingId);

                    if (referenceCount > 1)
                    {
                        // Create a separate configuration to avoid affecting other mappings
                        var newRange = new TblAllCollegesMapping
                        {
                            FromLetter = model.FromLetter.ToUpper().Trim(),
                            ToLetter = model.ToLetter.ToUpper().Trim(),
                            FromCollegeCode = currentRange.FromCollegeCode,
                            ToCollegeCode = currentRange.ToCollegeCode,
                            IsActive = model.IsActive,
                            CreatedDate = DateTime.Now,
                            CreatedBy = User.Identity?.Name
                        };

                        _context.TblAllCollegesMappings.Add(newRange);

                        existing.AllCollegesMapping = newRange;
                    }
                    else
                    {
                        // Update the existing configuration
                        currentRange.FromLetter = model.FromLetter.ToUpper().Trim();
                        currentRange.ToLetter = model.ToLetter.ToUpper().Trim();
                        currentRange.IsActive = model.IsActive;
                        currentRange.ModifiedDate = DateTime.Now;
                        currentRange.ModifiedBy = User.Identity?.Name;
                    }
                }

                // Update the main user mapping
                existing.FacultyCode = model.FacultyCode;
                existing.IsActive = model.IsActive;

                existing.VerificationLevel = model.NextVerificationLevel.HasValue
                    ? model.NextVerificationLevel.Value
                    : model.VerificationLevel;

                await _context.SaveChangesAsync();

                // Keep session in sync for the mapped user
                if (existing.UserId > 0)
                {
                    HttpContext.Session.SetInt32( "VerificationLevel", existing.VerificationLevel);
                }

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
        [Authorize(Roles = "Admin,ViceChancellor,Director")]
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
                .Where(e => e.Status == "Active")
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

        // GET: /CollegeMapping/GetCollegesByFaculty?mappingId=X
        [HttpGet]
        public async Task<IActionResult> GetCollegesByFaculty(int mappingId)
        {
            // Get the mapping details
            var mapping = await _context.TblCollegeMappings
                .Include(m => m.AllCollegesMapping)
                .FirstOrDefaultAsync(m => m.Id == mappingId && m.IsActive);

            if (mapping == null || mapping.AllCollegesMapping == null)
                return NotFound("College mapping not found or inactive.");

            var allCollegeMapping = mapping.AllCollegesMapping;
            var facultyCode = mapping.FacultyCode.ToString();

            if (allCollegeMapping.SelectedId.HasValue)
            {
                var selectedId = allCollegeMapping.SelectedId.Value;

                var selectedCollegeCodes = await _context.TblAllCollegesMappings
                    .Where(e => e.SelectedId == selectedId && e.IsActive)
                    .Select(e => e.FromCollegeCode)
                    .Distinct()
                    .ToListAsync();

                if (!selectedCollegeCodes.Any())
                {
                    return Json(new List<object>());
                }


                var colleges = await _context.AffiliationCollegeMasters
                    .Where(c =>
                        c.FacultyCode == facultyCode &&
                        c.CollegeCode != null &&
                        c.CollegeName != null &&
                        selectedCollegeCodes.Contains(c.CollegeCode))
                    .OrderBy(c => c.CollegeName)
                    .Select(c => new
                    {
                        code = c.CollegeCode,
                        name = c.CollegeName ?? "",
                        town = c.CollegeTown ?? ""
                    })
                    .ToListAsync();

                return Json(colleges);
            }
            else
            {
                // ==========================================
                // SERIAL MAPPING
                // ==========================================

                var fromLetter = allCollegeMapping.FromLetter?.Trim();
                var toLetter = allCollegeMapping.ToLetter?.Trim();

                var collegeFrom = allCollegeMapping.FromCollegeCode?.Trim();
                var collegeTo = allCollegeMapping.ToCollegeCode?.Trim();

                // Validate serial mapping ranges
                if (string.IsNullOrWhiteSpace(fromLetter) ||
                    string.IsNullOrWhiteSpace(toLetter) ||
                    string.IsNullOrWhiteSpace(collegeFrom) ||
                    string.IsNullOrWhiteSpace(collegeTo))
                {
                    return BadRequest("Invalid college mapping range.");
                }

                var facultyColleges = await _context.AffiliationCollegeMasters
                    .Where(c =>
                        c.FacultyCode == facultyCode &&
                        c.CollegeName != null &&
                        c.CollegeCode != null)
                    .ToListAsync();

                // Filter colleges using both letter and college-code ranges
                var colleges = facultyColleges
                    .Where(c =>
                    {
                        var collegeName = c.CollegeName?.Trim();
                        var collegeCode = c.CollegeCode?.Trim();

                        if (string.IsNullOrWhiteSpace(collegeName) || string.IsNullOrWhiteSpace(collegeCode))
                        {
                            return false;
                        }

                        var firstLetter = collegeName.Substring(0, 1);

                        // Check alphabetical range
                        bool letterMatches =
                            string.Compare(firstLetter, fromLetter, StringComparison.OrdinalIgnoreCase) >= 0
                            &&
                            string.Compare(firstLetter, toLetter, StringComparison.OrdinalIgnoreCase) <= 0;

                        // Check college-code range
                        bool codeMatches =
                            string.Compare(collegeCode, collegeFrom, StringComparison.OrdinalIgnoreCase) >= 0
                            &&
                            string.Compare(collegeCode, collegeTo, StringComparison.OrdinalIgnoreCase) <= 0;

                        return letterMatches && codeMatches;
                    })
                    .OrderBy(c => c.CollegeName)
                    .Select(c => new
                    {
                        code = c.CollegeCode,
                        name = c.CollegeName ?? "",
                        town = c.CollegeTown ?? ""
                    })
                    .ToList();

                return Json(colleges);
            }
        
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

            var facultyCode = facultyId.ToString();

            var colleges = await _context.AffiliationCollegeMasters
                .Where(c =>
                    c.FacultyCode == facultyCode &&
                    c.CollegeName != null &&
                    c.CollegeName.Trim() != "" &&
                    c.CollegeCode != null &&
                    c.CollegeCode.Trim() != "")
                .ToListAsync();

            var filteredColleges = colleges
                .Where(c =>
                {
                    var collegeName = c.CollegeName!.Trim();
                    var firstLetter = char.ToUpperInvariant(collegeName[0]);

                    return firstLetter >= fromLetter[0] &&
                           firstLetter <= toLetter[0];
                })
                .OrderBy(c => c.CollegeName)
                .Select(c => new
                {
                    code = c.CollegeCode,
                    name = c.CollegeName ?? "",
                    town = c.CollegeTown ?? ""
                })
                .ToList();

            return Json(filteredColleges);
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
