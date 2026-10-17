using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Controllers
{
    public class SectionWiseTabWiseController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IVerificationService _verificationService;
        private readonly IClinicalFacilitiesCompositeService _clinicalFacilitiesCompositeService;
        private static readonly int?[] _yearIds = { 1, 2, 3, 4 };

        public SectionWiseTabWiseController(ApplicationDbContext context, IVerificationService verificationService, IClinicalFacilitiesCompositeService clinicalFacilitiesCompositeService)
        {
            _context = context;
            _verificationService = verificationService;
            _clinicalFacilitiesCompositeService = clinicalFacilitiesCompositeService;
        }


        protected string BaseMedicalPath
        {
            get
            {
                return Directory.Exists(@"E:\")
                    ? @"E:\Affiliation_Medical"
                    : @"D:\Affiliation_Medical";
            }
        }

        protected string BaseDentalPath
        {
            get
            {
                return Directory.Exists(@"E:\")
                    ? @"E:\Affiliation_Dental"
                    : @"D:\Affiliation_Dental";
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSectionFeedback(SectionFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var referer = Request.Headers.Referer.ToString();

                if (!string.IsNullOrWhiteSpace(referer))
                {
                    var uri = new Uri(referer);
                    var localUrl = uri.PathAndQuery;

                    if (Url.IsLocalUrl(localUrl))
                    {
                        return Redirect(localUrl);
                    }
                }

                return RedirectToAction("InstitutionDetails", new { collegeCode = model.CollegeCode });
            }

            var courseLevel = HttpContext.Session.GetString("CourseLevel");

            var feedback = await _context.SectionWiseFeedbacks
                .FirstOrDefaultAsync(x =>
                    x.FacultyId == model.FacultyId &&
                    x.CollegeCode == model.CollegeCode &&
                    x.CourseLevel == courseLevel &&
                    x.TabId == model.TabId &&
                    x.SectionId == model.SectionId);

            bool isNew = feedback == null;

            if (feedback == null)
            {
                feedback = new SectionWiseFeedback
                {
                    FacultyId = model.FacultyId,
                    CollegeCode = model.CollegeCode,
                    TabId = model.TabId,
                    SectionId = model.SectionId,
                    CourseLevel = courseLevel,
                };

                _context.SectionWiseFeedbacks.Add(feedback);
            }

            feedback.VerificationStatus = model.VerificationStatus;
            feedback.Remarks = model.Remarks;
            feedback.VerifiedBy = User.Identity?.Name;
            feedback.VerifiedOn = DateTime.Now;
            feedback.CourseLevel = courseLevel;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = isNew ? "Section verification saved successfully." : "Section verification updated successfully.";

            // Redirect back to the page from which the form was submitted
            var refererUrl = Request.Headers.Referer.ToString();

            if (!string.IsNullOrWhiteSpace(refererUrl))
            {
                var uri = new Uri(refererUrl);
                var localUrl = uri.PathAndQuery;

                if (Url.IsLocalUrl(localUrl))
                {
                    return Redirect(localUrl);
                }
            }

            // Fallback
            return RedirectToAction("InstitutionDetails", new { collegeCode = model.CollegeCode });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSectionBySectionNameFeedback(SectionFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var referer = Request.Headers.Referer.ToString();

                if (!string.IsNullOrWhiteSpace(referer))
                {
                    var uri = new Uri(referer);
                    var localUrl = uri.PathAndQuery;

                    if (Url.IsLocalUrl(localUrl))
                    {
                        return Redirect(localUrl);
                    }
                }

                return RedirectToAction("InstitutionDetails",new { collegeCode = model.CollegeCode });
            }

            var courseLevel = HttpContext.Session.GetString("CourseLevel");

            if (string.IsNullOrWhiteSpace(courseLevel))
                return RedirectToAction("InstitutionDetails", new { collegeCode = model.CollegeCode });

            var tab = await _context.MstTabs.AsNoTracking().FirstOrDefaultAsync(e => e.TabName == model.TabName && e.FacultyId == model.FacultyId);

            if (tab == null)
            {
                TempData["ErrorMessage"] = "Invalid tab specified.";

                return RedirectToAction(nameof(InstitutionDetail), new { collegeCode = model.CollegeCode });
            }


            var feedback = await _context.SectionWiseFeedbacks
                .FirstOrDefaultAsync(x =>
                    x.FacultyId == model.FacultyId &&
                    x.CollegeCode == model.CollegeCode &&
                    x.CourseLevel == courseLevel &&
                    x.TabId == tab.TabId &&
                    x.SectionId == model.SectionId);

            bool isNew = feedback == null;

            if (feedback == null)
            {
                feedback = new SectionWiseFeedback
                {
                    FacultyId = model.FacultyId,
                    CollegeCode = model.CollegeCode,
                    TabId = model.TabId,
                    SectionId = model.SectionId,
                    CourseLevel = courseLevel,
                };

                _context.SectionWiseFeedbacks.Add(feedback);
            }

            feedback.VerificationStatus = model.VerificationStatus;
            feedback.Remarks = model.Remarks;
            feedback.VerifiedBy = User.Identity?.Name;
            feedback.VerifiedOn = DateTime.Now;
            feedback.CourseLevel = courseLevel;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = isNew ? "Section verification saved successfully." : "Section verification updated successfully.";

            // Redirect back to the page from which the form was submitted
            var refererUrl = Request.Headers.Referer.ToString();

            if (!string.IsNullOrWhiteSpace(refererUrl))
            {
                var uri = new Uri(refererUrl);
                var localUrl = uri.PathAndQuery;

                if (Url.IsLocalUrl(localUrl))
                {
                    return Redirect(localUrl);
                }
            }

            // Fallback
            return RedirectToAction( "InstitutionDetails", new { collegeCode = model.CollegeCode });
        }

    }
}
