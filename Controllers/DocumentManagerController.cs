using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.ViewModels;

namespace VerificationPortal.Controllers
{
    public class DocumentManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocumentManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET: DocumentManager
        // Displays:
        // 1. Faculty dropdown
        // 2. Add document form
        // 3. List of already added documents
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new DocumentManagerViewModel();

            // Load all faculties for the Faculty dropdown
            model.Faculties = await _context.Faculties
                .OrderBy(x => x.FacultyName)
                .ToListAsync();

            // Load all added documents
            model.Documents = await (
                from document in _context.MstDocuments

                join faculty in _context.Faculties
                    on document.FacultyId equals faculty.FacultyId

                join tab in _context.MstTabs
                    on document.TabId equals tab.TabId

                // Left join because SectionId can be NULL
                join section in _context.MstSections
                    on document.SectionId equals section.SectionId
                    into sectionGroup

                from section in sectionGroup.DefaultIfEmpty()

                orderby document.DisplayOrder,
                        document.DocumentName

                select new DocumentListViewModel
                {
                    DocumentId = document.DocumentId,
                    FacultyId = document.FacultyId,
                    FacultyName = faculty.FacultyName,
                    TabName = tab.TabName,
                    TabId = tab.TabId,

                    SectionName = section != null
                        ? section.SectionName
                        : null,

                    SectionId = section.SectionId,

                    DocumentName = document.DocumentName,
                    IsMandatory = document.IsMandatory,
                    DisplayOrder = document.DisplayOrder
                }

            ).ToListAsync();

            return View(model);
        }


        // ============================================================
        // GET: DocumentManager/GetTabs
        // Returns tabs based on the selected Faculty
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetTabs(int facultyId)
        {
            var tabs = await _context.MstTabs
                .Where(x => x.FacultyId == facultyId)
                .OrderBy(x => x.TabName)
                .Select(x => new
                {
                    x.TabId,
                    x.TabName
                })
                .ToListAsync();

            return Json(tabs);
        }


        // ============================================================
        // GET: DocumentManager/GetSections
        // Returns sections based on the selected Tab
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetSections(int tabId)
        {
            var sections = await _context.MstSections
                .Where(x => x.TabId == tabId)
                .OrderBy(x => x.SectionName)
                .Select(x => new
                {
                    x.SectionId,
                    x.SectionName
                })
                .ToListAsync();

            return Json(sections);
        }


        // ============================================================
        // POST: DocumentManager/Create
        // Saves a new verification document
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentManagerViewModel model)
        {
            // Validate required selections
            if (model.FacultyId <= 0)
            {
                ModelState.AddModelError( nameof(model.FacultyId), "Please select a faculty.");
            }

            if (model.TabId <= 0)
            {
                ModelState.AddModelError( nameof(model.TabId), "Please select a tab.");
            }

            if (string.IsNullOrWhiteSpace(model.DocumentName))
            {
                ModelState.AddModelError( nameof(model.DocumentName), "Please enter the document name.");
            }

            // If validation fails, reload the page data
            if (!ModelState.IsValid)
            {
                return await LoadIndexView(model);
            }

            // Check whether the same document already exists
            var documentExists = await _context.MstDocuments.AnyAsync(x =>
                x.FacultyId == model.FacultyId &&
                x.TabId == model.TabId &&
                x.SectionId == model.SectionId &&
                x.DocumentName.ToLower() == model.DocumentName.ToLower());

            if (documentExists)
            {
                ModelState.AddModelError(nameof(model.DocumentName),"This document already exists for the selected faculty, tab, and section.");
                return await LoadIndexView(model);
            }

            // Create the database entity
            var document = new MstDocument
            {
                FacultyId = model.FacultyId,
                TabId = model.TabId,
                SectionId = model.SectionId,
                DocumentName = model.DocumentName.Trim(),
                IsMandatory = model.IsMandatory
            };

            // Save to database
            _context.MstDocuments.Add(document);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Document added successfully.";

            return RedirectToAction(nameof(Index));
        }


        // ============================================================
        // LOAD INDEX VIEW
        // Loads all data required by the Document Manager page.
        //
        // This method is used when:
        // 1. Index page is opened
        // 2. Create validation fails
        // ============================================================
        private async Task<IActionResult> LoadIndexView( DocumentManagerViewModel model)
        {
            // Load Faculty dropdown
            model.Faculties = await _context.Faculties
                .OrderBy(x => x.FacultyName)
                .ToListAsync();


            // If Faculty is selected, load related Tabs
            if (model.FacultyId > 0)
            {
                model.Tabs = await _context.MstTabs
                    .Where(x => x.FacultyId == model.FacultyId)
                    .OrderBy(x => x.TabName)
                    .ToListAsync();
            }


            // If Tab is selected, load related Sections
            if (model.TabId > 0)
            {
                model.Sections = await _context.MstSections
                    .Where(x => x.TabId == model.TabId)
                    .OrderBy(x => x.SectionName)
                    .ToListAsync();
            }


            // Load all configured documents
            model.Documents = await (
                from document in _context.MstDocuments

                join faculty in _context.Faculties
                    on document.FacultyId equals faculty.FacultyId

                join tab in _context.MstTabs
                    on document.TabId equals tab.TabId

                // Left join because SectionId can be null
                join section in _context.MstSections
                    on document.SectionId equals section.SectionId
                    into sectionGroup

                from section in sectionGroup.DefaultIfEmpty()

                orderby document.DocumentName

                select new DocumentListViewModel
                {
                    DocumentId = document.DocumentId,

                    FacultyName = faculty.FacultyName,

                    TabName = tab.TabName,

                    SectionName = section != null
                        ? section.SectionName
                        : null,

                    DocumentName = document.DocumentName,

                    IsMandatory = document.IsMandatory
                }

            ).ToListAsync();

            return View("Index", model);
        }


        //=============================================================
        // POST : DocumentManager/Edit
        // UPDATES THE SELECTED VERIFICATION DOCUMENT
        //=============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentManagerViewModel model)
        {

            if(model.FacultyId <= 0)
                ModelState.AddModelError(nameof(model.FacultyId), "Please Select a faculty.");

            if (model.TabId <= 0)
                ModelState.AddModelError(nameof(model.TabId), "Please select a Tab.");

            if (string.IsNullOrWhiteSpace(model.DocumentName))
                ModelState.AddModelError(nameof(model.DocumentName), "Please enter the Document Name.");


            var documentName = model.DocumentName.Trim();

            var existingDocumentData = await _context.MstDocuments.FirstOrDefaultAsync(e=>e.DocumentId ==id);

            if (existingDocumentData == null)
            {
                TempData["ErrorMessage"] = "Document Not found.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Faculties = await _context.Faculties.OrderBy(e => e.FacultyName).ToListAsync();

                model.Tabs = await _context.MstTabs.Where(e=>e.FacultyId== model.FacultyId).OrderBy(e=>e.TabName).ToListAsync();

                model.Sections = await _context.MstSections.Where(e => e.TabId == model.TabId).OrderBy(e => e.SectionName).ToListAsync();

                ViewBag.DocumenId = id;
                return View(model);
            }

            //--------------------------------------------------------------
            // CHECK FOR DUPLICATE DOCUMENT EXCLUDING THE CURRENT RECORD
            // A document is considered duplicate when:
            // 1. Faculty is the same
            // 2. Tab is the same
            // 3. Section is the same, including null sections
            // 4. Document name matches ignoring case
            // 5. Current document is excluded
            //--------------------------------------------------------------

            var duplicateExists = await _context.MstDocuments.AnyAsync(e => 
                e.DocumentId != id &&
                e.FacultyId == model.FacultyId &&
                e.TabId == model.TabId && 
                e.SectionId == model.SectionId && 
                e.DocumentName.ToLower() == documentName.ToLower()
            );


            if (duplicateExists)
            {
                ModelState.AddModelError(nameof(model.DocumentName), "This Document already Exists.");
                return View(model);
            }


            existingDocumentData.FacultyId = model.FacultyId;
            existingDocumentData.TabId = model.TabId;
            existingDocumentData.DocumentName = documentName;
            existingDocumentData.IsMandatory = model.IsMandatory;
            existingDocumentData.SectionId = model.SectionId;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Document updated successfully";

            return RedirectToAction(nameof(Index));


        }


        //=============================================================
        // POST : DocumentManager/Delete/5
        // Permanently removes the selected document
        //=============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _context.MstDocuments
                .FirstOrDefaultAsync(e => e.DocumentId == id);

            if(document == null)
            {
                TempData["ErrorMessage"] = "Document not found";
                return RedirectToAction(nameof(Index));
            }

            _context.MstDocuments.Remove(document);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Document deleted successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentFeedback(int documentId, int facultyId, string collegeCode)
        {
            // ---------------------------------------------------------
            // VALIDATION
            // ---------------------------------------------------------

            if (documentId <= 0) return BadRequest("Invalid Document.");
            if (facultyId <= 0) return BadRequest("Invalid faculty.");
            if (string.IsNullOrWhiteSpace(collegeCode)) return BadRequest("College code is required.");

            var userId = GetUserId();

            // ---------------------------------------------------------
            // GET EXISTING FEEDBACK
            // ---------------------------------------------------------

            var feedback =
                await _context.DocumentWiseFeedbacks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.DocumentId == documentId &&
                        x.FacultyId == facultyId &&
                        x.CollegeCode == collegeCode &&
                        x.UserId == userId &&
                        x.IsActive);


            // ---------------------------------------------------------
            // NO FEEDBACK EXISTS
            // ---------------------------------------------------------

            if (feedback == null)
            {
                return Json(new
                {
                    exists = false,
                    status = "",
                    feedback = ""
                });
            }


            // ---------------------------------------------------------
            // RETURN EXISTING FEEDBACK
            // ---------------------------------------------------------

            return Json(new
            {
                exists = true,
                status = feedback.Status,
                feedback = feedback.Feedback
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDocumentFeedback( DocumentFeedbackViewModel model)
        {
            // ---------------------------------------------------------
            // VALIDATION
            // ---------------------------------------------------------

            if (model.DocumentId <= 0)
            {
                ModelState.AddModelError( nameof(model.DocumentId), "Invalid document.");
            }

            if (model.FacultyId <= 0)
            {
                ModelState.AddModelError( nameof(model.FacultyId), "Invalid faculty.");
            }

            if (string.IsNullOrWhiteSpace(model.CollegeCode))
            {
                ModelState.AddModelError( nameof(model.CollegeCode), "College code is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Status))
            {
                ModelState.AddModelError( nameof(model.Status), "Please select a verification status.");
            }


            if (!ModelState.IsValid)
            {
                return BadRequest( ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }


            // ---------------------------------------------------------
            // CURRENT USER
            // ---------------------------------------------------------

            var userId = GetUserId();


            // ---------------------------------------------------------
            // CHECK EXISTING FEEDBACK
            // ---------------------------------------------------------

            var existingFeedback =
                await _context.DocumentWiseFeedbacks
                    .FirstOrDefaultAsync(x =>
                        x.DocumentId == model.DocumentId &&
                        x.FacultyId == model.FacultyId &&
                        x.CollegeCode == model.CollegeCode &&
                        x.UserId == userId);


            // ---------------------------------------------------------
            // CREATE NEW FEEDBACK
            // ---------------------------------------------------------

            if (existingFeedback == null)
            {
                var newFeedback =
                    new DocumentWiseFeedback
                    {
                        DocumentId = model.DocumentId,
                        FacultyId = model.FacultyId,
                        CollegeCode = model.CollegeCode,
                        UserId = userId,

                        Feedback = model.Feedback,
                        Status = model.Status,

                        IsActive = true,
                        CreatedOn = DateTime.Now
                    };

                _context.DocumentWiseFeedbacks.Add(
                    newFeedback);
            }

            // ---------------------------------------------------------
            // UPDATE EXISTING FEEDBACK
            // ---------------------------------------------------------

            else
            {
                existingFeedback.Feedback = model.Feedback;
                existingFeedback.Status = model.Status;
                existingFeedback.IsActive = true;
                existingFeedback.ModifiedOn = DateTime.Now;
            }


            // ---------------------------------------------------------
            // SAVE
            // ---------------------------------------------------------

            await _context.SaveChangesAsync();


            return Json(new
            {
                success = true,
                message = "Document feedback saved successfully."
            });
        }


        // ============================================================
        // GET CURRENT LOGGED-IN USER ID
        // PURPOSE:
        // Retrieves the primary key of the currently authenticated user
        // from the authentication claims.
        //
        // This value corresponds to:
        //
        // TblRguhsFacultyUser.Id
        // ============================================================

        private int GetUserId()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                throw new UnauthorizedAccessException( "Unable to determine the current user.");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException( "Invalid user ID in authentication claims.");
            }

            return userId;
        }

    }
}