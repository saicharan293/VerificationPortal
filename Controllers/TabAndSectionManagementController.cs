using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Models.TabManagementViewModel;

namespace VerificationPortal.Controllers
{
    public class TabAndSectionManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TabAndSectionManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // MAIN INDEX
        // ==========================================

        public async Task<IActionResult> Index(int? facultyId, int? tabId)
        {
            var tabsQuery = _context.MstTabs
                .Include(x => x.Faculty)
                .Include(x => x.MstSections)
                .AsQueryable();

            var sectionsQuery = _context.MstSections
                .Include(x => x.Faculty)
                .Include(x => x.Tab)
                .AsQueryable();

            if (facultyId.HasValue)
            {
                tabsQuery = tabsQuery.Where(x => x.FacultyId == facultyId);
                sectionsQuery = sectionsQuery.Where(x => x.FacultyId == facultyId);
            }

            if (tabId.HasValue)
            {
                tabsQuery = tabsQuery.Where(x => x.TabId == tabId);
                sectionsQuery = sectionsQuery.Where(x => x.TabId == tabId);
            }

            var model = new TabAndSectionManagementViewModel
            {
                Tabs = await tabsQuery
                    .OrderBy(x => x.Faculty.FacultyName)
                    .ThenBy(x => x.TabName)
                    .Select(x => new TabViewModel
                    {
                        TabId = x.TabId,
                        FacultyId = x.FacultyId,
                        FacultyName = x.Faculty.FacultyName,
                        TabName = x.TabName,
                        TotalSections = x.MstSections.Count()
                    })
                    .ToListAsync(),

                Sections = await sectionsQuery
                    .OrderBy(x => x.Tab.TabName)
                    .ThenBy(x => x.SectionName)
                    .Select(x => new SectionManagementViewModel
                    {
                        SectionId = x.SectionId,
                        FacultyId = x.FacultyId,
                        FacultyName = x.Faculty.FacultyName,
                        TabId = x.TabId,
                        TabName = x.Tab.TabName,
                        SectionName = x.SectionName
                    })
                    .ToListAsync(),

                SelectedFacultyId = facultyId,
                SelectedTabId = tabId
            };

            if (facultyId.HasValue)
            {
                model.SelectedFacultyName = await _context.Faculties
                    .Where(x => x.FacultyId == facultyId)
                    .Select(x => x.FacultyName)
                    .FirstOrDefaultAsync();
            }

            if (tabId.HasValue)
            {
                model.SelectedTabName = await _context.MstTabs
                    .Where(x => x.TabId == tabId)
                    .Select(x => x.TabName)
                    .FirstOrDefaultAsync();
            }

            ViewBag.Faculties = new SelectList(
                await _context.Faculties
                    .OrderBy(x => x.FacultyName)
                    .ToListAsync(),
                "FacultyId",
                "FacultyName",
                facultyId
            );

            ViewBag.Tabs = new SelectList(
                await _context.MstTabs
                    .Where(x => !facultyId.HasValue || x.FacultyId == facultyId)
                    .OrderBy(x => x.TabName)
                    .ToListAsync(),
                "TabId",
                "TabName",
                tabId
            );

            return View(model);
        }

        // ==========================================
        // TAB MANAGEMENT
        // ==========================================

        // GET: Create Tab
        [HttpGet]
        public async Task<IActionResult> CreateTab()
        {
            var model = new TabViewModel();

            await PopulateFacultiesAsync(model);

            return View(model);
        }

        // POST: Create Tab
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTab(TabViewModel model)
        {
            if (model.FacultyId <= 0 ||
                !await _context.Faculties.AnyAsync(x => x.FacultyId == model.FacultyId))
            {
                ModelState.AddModelError(nameof(model.FacultyId),
                    "Please select a valid faculty.");
            }

            if (!string.IsNullOrWhiteSpace(model.TabName))
            {
                model.TabName = model.TabName.Trim();

                bool exists = await _context.MstTabs.AnyAsync(x =>
                    x.FacultyId == model.FacultyId &&
                    x.TabName == model.TabName);

                if (exists)
                {
                    ModelState.AddModelError(nameof(model.TabName),
                        "This tab already exists for the selected faculty.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateFacultiesAsync(model);
                return View(model);
            }

            var tab = new MstTab
            {
                FacultyId = model.FacultyId,
                TabName = model.TabName
            };

            _context.MstTabs.Add(tab);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tab created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit Tab
        [HttpGet]
        public async Task<IActionResult> EditTab(int id)
        {
            var tab = await _context.MstTabs
                .FirstOrDefaultAsync(x => x.TabId == id);

            if (tab == null)
                return NotFound();

            var model = new TabViewModel
            {
                TabId = tab.TabId,
                FacultyId = tab.FacultyId,
                TabName = tab.TabName
            };

            await PopulateFacultiesAsync(model);

            return View(nameof(CreateTab), model);
        }

        // POST: Edit Tab
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTab(TabViewModel model)
        {
            var tab = await _context.MstTabs
                .FirstOrDefaultAsync(x => x.TabId == model.TabId);

            if (tab == null)
                return NotFound();

            if (model.FacultyId <= 0 ||
                !await _context.Faculties.AnyAsync(x => x.FacultyId == model.FacultyId))
            {
                ModelState.AddModelError(nameof(model.FacultyId),
                    "Please select a valid faculty.");
            }

            if (!string.IsNullOrWhiteSpace(model.TabName))
            {
                model.TabName = model.TabName.Trim();

                bool exists = await _context.MstTabs.AnyAsync(x =>
                    x.FacultyId == model.FacultyId &&
                    x.TabName == model.TabName &&
                    x.TabId != model.TabId);

                if (exists)
                {
                    ModelState.AddModelError(nameof(model.TabName),
                        "This tab already exists for the selected faculty.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateFacultiesAsync(model);
                return View(nameof(CreateTab), model);
            }

            tab.FacultyId = model.FacultyId;
            tab.TabName = model.TabName;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tab updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Tab
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTab(int id)
        {
            var tab = await _context.MstTabs
                .Include(x => x.MstSections)
                .Include(x => x.MstDocuments)
                .Include(x => x.TabWiseFeedbacks)
                .FirstOrDefaultAsync(x => x.TabId == id);

            if (tab == null)
                return NotFound();

            if (tab.MstSections.Any() ||
                tab.MstDocuments.Any() ||
                tab.TabWiseFeedbacks.Any())
            {
                TempData["ErrorMessage"] =
                    "Cannot delete this tab because it has associated sections, documents or feedback records.";

                return RedirectToAction(nameof(Index));
            }

            _context.MstTabs.Remove(tab);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tab deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // SECTION MANAGEMENT
        // ==========================================

        // GET: Create Section
        [HttpGet]
        public async Task<IActionResult> CreateSection(int? tabId)
        {
            var model = new SectionManagementViewModel
            {
                TabId = tabId ?? 0
            };

            await PopulateSectionDropdownsAsync(model);

            return View(model);
        }

        // POST: Create Section
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSection(SectionManagementViewModel model)
        {
            var selectedTab = await _context.MstTabs
                .FirstOrDefaultAsync(x => x.TabId == model.TabId);

            if (selectedTab == null ||
                selectedTab.FacultyId != model.FacultyId)
            {
                ModelState.AddModelError(nameof(model.TabId),
                    "Please select a valid tab for the selected faculty.");
            }

            if (!string.IsNullOrWhiteSpace(model.SectionName))
            {
                model.SectionName = model.SectionName.Trim();

                bool exists = await _context.MstSections.AnyAsync(x =>
                    x.TabId == model.TabId &&
                    x.SectionName == model.SectionName);

                if (exists)
                {
                    ModelState.AddModelError(nameof(model.SectionName),
                        "This section already exists under the selected tab.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateSectionDropdownsAsync(model);
                return View(model);
            }

            var section = new MstSection
            {
                FacultyId = model.FacultyId,
                TabId = model.TabId,
                SectionName = model.SectionName
            };

            _context.MstSections.Add(section);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Section created successfully.";

            return RedirectToAction(nameof(Index), new
            {
                facultyId = model.FacultyId,
                tabId = model.TabId
            });
        }

        // GET: Edit Section
        [HttpGet]
        public async Task<IActionResult> EditSection(int id)
        {
            var section = await _context.MstSections
                .FirstOrDefaultAsync(x => x.SectionId == id);

            if (section == null)
                return NotFound();

            var model = new SectionManagementViewModel
            {
                SectionId = section.SectionId,
                FacultyId = section.FacultyId,
                TabId = section.TabId,
                SectionName = section.SectionName
            };

            await PopulateSectionDropdownsAsync(model);

            return View(nameof(CreateSection), model);
        }

        // POST: Edit Section
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSection(SectionManagementViewModel model)
        {
            var section = await _context.MstSections
                .FirstOrDefaultAsync(x => x.SectionId == model.SectionId);

            if (section == null)
                return NotFound();

            var selectedTab = await _context.MstTabs
                .FirstOrDefaultAsync(x => x.TabId == model.TabId);

            if (selectedTab == null ||
                selectedTab.FacultyId != model.FacultyId)
            {
                ModelState.AddModelError(nameof(model.TabId),
                    "Please select a valid tab for the selected faculty.");
            }

            if (!string.IsNullOrWhiteSpace(model.SectionName))
            {
                model.SectionName = model.SectionName.Trim();

                bool exists = await _context.MstSections.AnyAsync(x =>
                    x.TabId == model.TabId &&
                    x.SectionName == model.SectionName &&
                    x.SectionId != model.SectionId);

                if (exists)
                {
                    ModelState.AddModelError(nameof(model.SectionName),
                        "This section already exists under the selected tab.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateSectionDropdownsAsync(model);
                return View(nameof(CreateSection), model);
            }

            section.FacultyId = model.FacultyId;
            section.TabId = model.TabId;
            section.SectionName = model.SectionName;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Section updated successfully.";

            return RedirectToAction(nameof(Index), new
            {
                facultyId = model.FacultyId,
                tabId = model.TabId
            });
        }

        // POST: Delete Section
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var section = await _context.MstSections
                .Include(x => x.MstDocuments)
                .Include(x => x.SectionWiseFeedbacks)
                .FirstOrDefaultAsync(x => x.SectionId == id);

            if (section == null)
                return NotFound();

            int facultyId = section.FacultyId;
            int tabId = section.TabId;

            if (section.MstDocuments.Any() ||
                section.SectionWiseFeedbacks.Any())
            {
                TempData["ErrorMessage"] =
                    "Cannot delete this section because it has associated documents or feedback records.";

                return RedirectToAction(nameof(Index), new
                {
                    facultyId,
                    tabId
                });
            }

            _context.MstSections.Remove(section);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Section deleted successfully.";

            return RedirectToAction(nameof(Index), new
            {
                facultyId,
                tabId
            });
        }

        // ==========================================
        // AJAX: GET TABS BY FACULTY
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetTabsByFaculty(int facultyId)
        {
            var tabs = await _context.MstTabs
                .Where(x => x.FacultyId == facultyId)
                .OrderBy(x => x.TabName)
                .Select(x => new
                {
                    value = x.TabId,
                    text = x.TabName
                })
                .ToListAsync();

            return Json(tabs);
        }

        // ==========================================
        // DROPDOWN HELPERS
        // ==========================================

        private async Task PopulateFacultiesAsync(TabViewModel model)
        {
            model.Faculties = await _context.Faculties
                .OrderBy(x => x.FacultyName)
                .Select(x => new SelectListItem
                {
                    Value = x.FacultyId.ToString(),
                    Text = x.FacultyName,
                    Selected = x.FacultyId == model.FacultyId
                })
                .ToListAsync();
        }

        private async Task PopulateSectionDropdownsAsync(
            SectionManagementViewModel model)
        {
            model.Faculties = await _context.Faculties
                .OrderBy(x => x.FacultyName)
                .Select(x => new SelectListItem
                {
                    Value = x.FacultyId.ToString(),
                    Text = x.FacultyName,
                    Selected = x.FacultyId == model.FacultyId
                })
                .ToListAsync();

            model.Tabs = await _context.MstTabs
                .Where(x => x.FacultyId == model.FacultyId)
                .OrderBy(x => x.TabName)
                .Select(x => new SelectListItem
                {
                    Value = x.TabId.ToString(),
                    Text = x.TabName,
                    Selected = x.TabId == model.TabId
                })
                .ToListAsync();
        }
    }
}