using Microsoft.EntityFrameworkCore;
using VerificationPortal.Controllers;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Models.ViewModels;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class CollegeMappingService : ICollegeMappingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CollegeMappingController> _logger;
        public CollegeMappingService(ApplicationDbContext context, ILogger<CollegeMappingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CollegeMappingListViewModel> GetMappingsAsync(string? search, int? facultyId)
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

            return new CollegeMappingListViewModel
            {
                Mappings = mappingList,
                TotalMappings = await _context.TblCollegeMappings.CountAsync(),
                ActiveMappings = await _context.TblCollegeMappings.CountAsync(m => m.IsActive),

                TotalUsers = await _context.TblCollegeMappings.Select(m => m.UserId).Distinct().CountAsync()
            };
        }

        public async Task<CollegeMappingCreateViewModel> GetCreateViewModelAsync(int? facultyId)
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

            return model;
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

            return await query.OrderBy(u => u.Faculty).ThenBy(u => u.UserName).ToListAsync();
        }

        public async Task<string?> CreateMappingAsync(CollegeMappingCreateViewModel model, string createdBy)
        {
            var user = await _context.TblRguhsFacultyUsers.FirstOrDefaultAsync(u =>
                 u.UserId == model.SelectedUserId &&
                 u.Faculty == model.SelectedFacultyId);

            if (user == null)
                return "Selected user not found.";

            // 2. Prevent duplicate active mapping
            var existingUserMapping = await _context.TblCollegeMappings
                .FirstOrDefaultAsync(m =>
                    m.UserId == user.UserId &&
                    m.FacultyCode == model.SelectedFacultyId &&
                    m.IsActive);

            if (existingUserMapping != null)
            {
                return "This user already has an active college mapping for this faculty. " +
                       "Please edit the existing mapping instead.";
            }

            // 3. Get selected college codes
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
                return "No valid colleges selected.";

            // 4. Validate all submitted college codes
            if (selectedColleges.Count != selectedCollegeCodes.Count)
                return "One or more selected colleges are invalid.";

            // 5. Determine whether selected colleges are serial
            var orderedCollegeCodes = await _context.AffiliationCollegeMasters
                .Where(e =>
                    e.FacultyCode == model.SelectedFacultyId.ToString() &&
                    e.Status == true &&
                    e.CollegeCode != null)
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

            bool isSerial =
                selectedIndexes.Last() - selectedIndexes.First() + 1
                == selectedIndexes.Count;

            // 6. Arrange selected colleges in order
            var selectedCollegesByCode = selectedColleges
                .ToDictionary( e => e.CollegeCode, StringComparer.OrdinalIgnoreCase);

            var orderedSelectedColleges = selectedIndexes
                .Select(index => selectedCollegesByCode[orderedCollegeCodes[index]])
                .ToList();

            // 7. Check overlap only for non-admin users
            if (user.IsAdmin != true)
            {
                var existingMappings = await _context.TblCollegeMappings
                    .Include(m => m.AllCollegesMapping)
                    .Where(m =>
                        m.FacultyCode == model.SelectedFacultyId &&
                        m.IsActive &&
                        m.UserId != user.UserId)
                    .ToListAsync();

                // Find users associated with existing mappings
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

                // Exclude Admin mappings
                existingMappings = existingMappings
                    .Where(m => !adminUserIds.Contains(m.UserId))
                    .ToList();

                // Get colleges for the selected faculty
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

                    if (range == null)
                        continue;

                    var existingColleges = facultyColleges
                        .Where(c =>
                        {
                            var collegeName = c.CollegeName?.Trim();

                            if (string.IsNullOrEmpty(collegeName))
                                return false;

                            var firstLetter = collegeName.Substring(0, 1);

                            return
                                string.Compare(
                                    firstLetter,
                                    range.FromLetter,
                                    StringComparison.OrdinalIgnoreCase) >= 0
                                &&
                                string.Compare(
                                    firstLetter,
                                    range.ToLetter,
                                    StringComparison.OrdinalIgnoreCase) <= 0;
                        })
                        .Where(c =>
                            string.Compare(
                                c.CollegeCode,
                                range.FromCollegeCode,
                                StringComparison.OrdinalIgnoreCase) >= 0
                            &&
                            string.Compare(
                                c.CollegeCode,
                                range.ToCollegeCode,
                                StringComparison.OrdinalIgnoreCase) <= 0)
                        .ToList();

                    if (existingColleges.Any(c =>
                        selectedCollegeCodes.Contains(
                            c.CollegeCode,
                            StringComparer.OrdinalIgnoreCase)))
                    {
                        overlappingMappings.Add(existingMapping);
                    }
                }

                // Your original overlap rejection block was commented out.
                // So this version also calculates overlaps without rejecting them.
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

            // 8. Create TblAllCollegesMapping records
            TblAllCollegesMapping allCollegesMapping;

            if (isSerial)
            {
                // SERIAL: save one range record
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
                    CreatedBy = createdBy
                };

                _context.TblAllCollegesMappings.Add(allCollegesMapping);
            }
            else
            {
                // SELECTIVE: save one record per college
                var maxSelectedId = await _context.TblAllCollegesMappings
                    .Where(x => x.SelectedId.HasValue)
                    .MaxAsync(x => (int?)x.SelectedId) ?? 0;

                var selectedId = maxSelectedId + 1;

                var selectedMappingRecords =
                    new List<TblAllCollegesMapping>();

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
                        CreatedBy = createdBy
                    };

                    selectedMappingRecords.Add(selectedMapping);
                }

                _context.TblAllCollegesMappings.AddRange(
                    selectedMappingRecords);

                // First record becomes the parent reference
                allCollegesMapping = selectedMappingRecords.First();
            }

            // 9. Determine verification level
            var maxLevel = await _context.TblCollegeMappings
                .Where(m => m.FacultyCode == model.SelectedFacultyId)
                .MaxAsync(m => (int?)m.VerificationLevel) ?? 0;

            // 10. Create TblCollegeMapping parent record
            var mapping = new TblCollegeMapping
            {
                UserId = user.UserId,
                UserName = user.UserName ?? "",
                FacultyCode = model.SelectedFacultyId,

                AllCollegesMapping = allCollegesMapping,

                CreatedDate = DateTime.Now,
                CreatedBy = createdBy,
                IsActive = true,

                VerificationLevel = model.NextVerificationLevel.HasValue
                    ? model.NextVerificationLevel.Value
                    : (model.VerificationLevel.HasValue &&
                       model.VerificationLevel > 0
                        ? model.VerificationLevel.Value
                        : maxLevel + 1)
            };

            _context.TblCollegeMappings.Add(mapping);

            // 11. Save database changes
            await _context.SaveChangesAsync();

            // 12. Logging
            _logger.LogInformation(
                "User {UserId} ({UserName}) assigned {Count} colleges. Serial={IsSerial}, IsAdmin={IsAdmin}",
                user.UserId,
                user.UserName,
                selectedColleges.Count,
                isSerial,
                user.IsAdmin);

            // null means success
            return null;
        }


        public async Task PopulateCreateViewModelAsync(CollegeMappingCreateViewModel model)
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

        public async Task<bool> MappingExists(int id)
        {
            return await _context.TblCollegeMappings.AnyAsync(m => m.Id == id);
        }


        public async Task<CollegeMappingEditViewModel?> GetEditViewModelAsync(int id)
        {
            var mapping = await _context.TblCollegeMappings
                .Include(m => m.FacultyCodeNavigation)
                .Include(m => m.AllCollegesMapping)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mapping == null)
                return null;

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

            return model;
        }


        public async Task PopulateEditViewModelAsync(CollegeMappingEditViewModel model)
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

        public async Task<bool> UpdateMappingAsync(int id, CollegeMappingEditViewModel model, string modifiedBy)
        {
            var existing = await _context.TblCollegeMappings
                .Include(m => m.AllCollegesMapping)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existing == null)
                return false;

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
                    CreatedBy = modifiedBy
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
                        CreatedBy = modifiedBy
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
                    currentRange.ModifiedBy = modifiedBy;
                }
            }

            // Update the main user mapping
            existing.FacultyCode = model.FacultyCode;
            existing.IsActive = model.IsActive;

            existing.VerificationLevel = model.NextVerificationLevel.HasValue
                ? model.NextVerificationLevel.Value
                : model.VerificationLevel;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteMappingAsync(int id)
        {
            var mapping = await _context.TblCollegeMappings
                .Include(m => m.AllCollegesMapping)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mapping == null)
                return false;

            var allCollegesMapping = mapping.AllCollegesMapping;

            // Remove the main mapping
            _context.TblCollegeMappings.Remove(mapping);

            // Check whether another mapping uses the same configuration
            if (allCollegesMapping != null)
            {
                var referenceCount = await _context.TblCollegeMappings
                    .CountAsync(m =>
                        m.AllCollegesMappingId == mapping.AllCollegesMappingId &&
                        m.Id != mapping.Id);

                // Delete configuration only if no other mapping uses it
                if (referenceCount == 0)
                {
                    if (allCollegesMapping.SelectedId.HasValue)
                    {
                        // Selective mapping: delete all rows in this group
                        var selectedId = allCollegesMapping.SelectedId.Value;

                        var relatedRows = await _context.TblAllCollegesMappings
                            .Where(m => m.SelectedId == selectedId)
                            .ToListAsync();

                        _context.TblAllCollegesMappings.RemoveRange(relatedRows);
                    }
                    else
                    {
                        // Serial mapping: delete its single configuration row
                        _context.TblAllCollegesMappings.Remove(allCollegesMapping);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<CollegeOptionViewModel>> GetCollegesByFacultyAsync(int mappingId)
        {
            var mapping = await _context.TblCollegeMappings
                .Include(m => m.AllCollegesMapping)
                .FirstOrDefaultAsync(m => m.Id == mappingId && m.IsActive);

            if (mapping == null || mapping.AllCollegesMapping == null)
                return null;

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
                    return new List<CollegeOptionViewModel>();
                }


                var colleges = await _context.AffiliationCollegeMasters
                    .Where(c =>
                        c.FacultyCode == facultyCode &&
                        c.CollegeCode != null &&
                        c.CollegeName != null &&
                        selectedCollegeCodes.Contains(c.CollegeCode))
                    .OrderBy(c => c.CollegeName)
                    .Select(c => new CollegeOptionViewModel
                    {
                        Code = c.CollegeCode,
                        Name = c.CollegeName ?? "",
                        Town = c.CollegeTown ?? ""
                    })
                    .ToListAsync();

                return colleges;
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
                    throw new InvalidOperationException("Invalid college mapping range.");
                }

                var facultyColleges = await _context.AffiliationCollegeMasters
                    .Where(c =>
                        c.FacultyCode == facultyCode &&
                        c.CollegeName != null &&
                        c.CollegeCode != null)
                    .ToListAsync();

                // Filter colleges using both letter and college-code ranges
                var serialcolleges = facultyColleges
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
                            string.Compare(firstLetter, fromLetter, StringComparison.OrdinalIgnoreCase) >= 0 &&
                            string.Compare(firstLetter, toLetter, StringComparison.OrdinalIgnoreCase) <= 0;

                        // Check college-code range
                        bool codeMatches =
                            string.Compare(collegeCode, collegeFrom, StringComparison.OrdinalIgnoreCase) >= 0 &&
                            string.Compare(collegeCode, collegeTo, StringComparison.OrdinalIgnoreCase) <= 0;

                        return letterMatches && codeMatches;
                    })
                    .OrderBy(c => c.CollegeName)
                    .Select(c => new CollegeOptionViewModel
                    {
                        Code = c.CollegeCode,
                        Name = c.CollegeName ?? "",
                        Town = c.CollegeTown ?? ""
                    })
                    .ToList();

                return serialcolleges;
            }
        }

        public async Task<List<CollegeOptionViewModel>> GetCollegesByFacultyIdAsync(int facultyId, string fromLetter, string toLetter)
        {
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

                    return firstLetter >= fromLetter[0] && firstLetter <= toLetter[0];
                })
                .OrderBy(c => c.CollegeName)
                .Select(c => new CollegeOptionViewModel
                {
                    Code = c.CollegeCode,
                    Name = c.CollegeName ?? "",
                    Town = c.CollegeTown ?? ""
                })
                .ToList();

            return filteredColleges;
        }
    }


}
