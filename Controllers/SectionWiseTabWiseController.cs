using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;

namespace VerificationPortal.Controllers
{
    [Authorize]
    public class SectionWiseTabWiseController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IVerificationService _verificationService;
        private readonly ILogger<SectionWiseTabWiseController> _logger;
        private readonly IClinicalFacilitiesCompositeService _clinicalFacilitiesCompositeService;
        private static readonly int?[] _yearIds = { 1, 2, 3, 4 };

        public SectionWiseTabWiseController(ApplicationDbContext context, IVerificationService verificationService, IClinicalFacilitiesCompositeService clinicalFacilitiesCompositeService, ILogger<SectionWiseTabWiseController> logger)
        {
            _context = context;
            _verificationService = verificationService;
            _clinicalFacilitiesCompositeService = clinicalFacilitiesCompositeService;
            _logger = logger;
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

        private IActionResult RedirectToReturnUrl(string? returnUrl, string fallbackAction, object? routeValues = null)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(fallbackAction, routeValues);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSectionBySectionNameFeedback(SectionFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToReturnUrl(model.ReturnUrl, nameof(InstitutionDetail), new { collegeCode = model.CollegeCode });
            }

            var courseLevel = HttpContext.Session.GetString("CourseLevel");

            if (string.IsNullOrWhiteSpace(courseLevel))
            {
                return RedirectToReturnUrl(model.ReturnUrl, nameof(InstitutionDetail), new { collegeCode = model.CollegeCode });
            }

            try
            {
                var connection = _context.Database.GetDbConnection();

                bool shouldCloseConnection = connection.State != ConnectionState.Open;

                if (shouldCloseConnection)
                {
                    await connection.OpenAsync();
                }
                try
                {
                    await using var command = connection.CreateCommand();

                    command.CommandText = "dbo.verification_SectionWiseFeedback";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new SqlParameter("@FacultyId", SqlDbType.Int) { Value = model.FacultyId });
                    command.Parameters.Add(new SqlParameter("@CollegeCode", SqlDbType.NVarChar, 50) { Value = model.CollegeCode });
                    command.Parameters.Add(new SqlParameter("@CourseLevel", SqlDbType.NVarChar, 50) { Value = courseLevel });
                    command.Parameters.Add(new SqlParameter("@TabName", SqlDbType.NVarChar, 255) { Value = model.TabName });
                    command.Parameters.Add(new SqlParameter("@SectionName", SqlDbType.NVarChar, 255) { Value = model.SectionName });
                    command.Parameters.Add(new SqlParameter("@VerificationStatus", SqlDbType.NVarChar, 50) { Value = (object?)model.VerificationStatus ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.NVarChar, -1) { Value = (object?)model.Remarks ?? DBNull.Value });
                    command.Parameters.Add(new SqlParameter("@VerifiedBy", SqlDbType.NVarChar, 255) { Value = (object?)User.Identity?.Name ?? DBNull.Value });

                    await using var reader = await command.ExecuteReaderAsync();

                    if(!await reader.ReadAsync())
                    {
                        TempData["ErrorMessage"] = "No result was returned from the feedback procedure.";
                        return RedirectToReturnUrl(model.ReturnUrl, nameof(InstitutionDetail), new { collegeCode = model.CollegeCode });
                    }

                    var result = new SectionFeedbackSaveResult
                    {
                        SaveStatus = reader["SaveStatus"]?.ToString() ?? "",
                        TabId = Convert.ToInt32(reader["TabId"]),
                        SectionId = Convert.ToInt32(reader["SectionId"])
                    };

                    TempData["SuccessMessage"] = result.SaveStatus == "Accepted" 
                        ? "Section verification saved successfully."
                        : "Section verification updated successfully.";
                }

                finally
                {
                    if (shouldCloseConnection) await connection.CloseAsync();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error saving section feedback for Collegecode {CollegeCode}, FacultyId {FacultyId}, TabName {TabName}, Section name {SectionName}",
                    model.CollegeCode,
                    model.FacultyId,
                    model.TabName,
                    model.SectionId
                );
                TempData["ErrorMessage"] = "Unable to save section verification";

                return RedirectToReturnUrl(model.ReturnUrl, nameof(InstitutionDetail), new { collegeCode = model.CollegeCode });
            }
           
            // Fallback
            return RedirectToReturnUrl(model.ReturnUrl, "InstitutionDetails", new { collegeCode = model.CollegeCode });
        }

    }
}
