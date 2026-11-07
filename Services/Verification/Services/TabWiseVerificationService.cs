using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class TabWiseVerificationService : ITabWiseVerificationService
    {
        private readonly ApplicationDbContext _context; 
        public TabWiseVerificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TabWiseVerificationResultVm> GetTabFeedbackByTabNameAsync(string collegeCode, string tabName, string currentUser, int? facultyId, string? courseLevel, bool isAdmin)
        {
            var result = new TabWiseVerificationResultVm
            {
                IsAdmin = isAdmin,
                ExistingStatus = "Pending",
                ExistingStatusClass = "bg-warning",
                ShowFeedbackForm = false
            };

            if (string.IsNullOrWhiteSpace(collegeCode) || string.IsNullOrWhiteSpace(tabName))
            {
                return result;
            }

            var tabQuery = _context.MstTabs.AsNoTracking().Where(x => x.TabName == tabName);

            if (facultyId.HasValue)
            {
                tabQuery = tabQuery.Where( x => x.FacultyId == facultyId.Value);
            }

            var tab = await tabQuery.FirstOrDefaultAsync();

            if (tab == null)
            {
                return result;
            }

            var feedbackQuery = _context.TabWiseFeedbacks
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.TabId == tab.TabId &&
                    x.VerifiedBy == currentUser);

            if (facultyId.HasValue)
            {
                feedbackQuery = feedbackQuery.Where( x => x.FacultyId == facultyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(courseLevel))
            {
                feedbackQuery = feedbackQuery.Where( x => x.CourseLevel == courseLevel);
            }

            var feedback = await feedbackQuery.FirstOrDefaultAsync();

            if (feedback == null)
            {
                return result;
            }

            result.HasFeedback = true;

            result.ExistingRemarks = feedback.Remarks;

            result.ExistingStatus =
                feedback.VerificationStatus?.ToLower() switch
                {
                    "accept" => "Approved",
                    "approved" => "Approved",

                    "reject" => "Rejected",
                    "rejected" => "Rejected",

                    "pending" => "Pending",

                    _ => "Pending"
                };


            // ---------------------------------------------------------
            // STATUS BADGE CLASS
            // ---------------------------------------------------------

            result.ExistingStatusClass =
                result.ExistingStatus switch
                {
                    "Approved" => "bg-success",
                    "Rejected" => "bg-danger",
                    _ => "bg-warning"
                };

            // ---------------------------------------------------------
            // VERIFICATION INFORMATION
            // ---------------------------------------------------------

            result.VerifiedBy = feedback.VerifiedBy;

            result.VerifiedDate = feedback.VerifiedOn?
                .ToString("dd-MM-yyyy hh:mm tt");

            result.ShowFeedbackForm =
                string.IsNullOrWhiteSpace(
                    feedback.VerificationStatus);

            result.TabId = feedback.TabId;

            result.TabName = tab.TabName;

            // ---------------------------------------------------------
            // VERIFICATION HISTORY - OTHER USERS
            // ---------------------------------------------------------

            var historyQuery = _context.TabWiseFeedbacks
                .AsNoTracking()
                .Where(x =>
                    x.CollegeCode == collegeCode &&
                    x.VerifiedBy != currentUser &&
                    x.TabId == tab.TabId);

            if (facultyId.HasValue)
            {
                historyQuery = historyQuery.Where(
                    x => x.FacultyId == facultyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(courseLevel))
            {
                historyQuery = historyQuery.Where(x => x.CourseLevel == courseLevel);
            }

            result.VerificationHistory = await historyQuery
                .Select(x => new VerificationHistoryViewModel
                {
                    Designation = x.VerifiedBy,

                    IsVerified =
                        x.VerificationStatus == "accept" ||
                        x.VerificationStatus == "approved"
                            ? true
                            : x.VerificationStatus == "reject" ||
                              x.VerificationStatus == "rejected"
                                ? false
                                : null,

                    Remarks = x.Remarks,

                    VerifiedBy = x.VerifiedBy,

                    VerifiedDate = x.VerifiedOn
                })
                .OrderByDescending(x => x.VerifiedDate)
                .ToListAsync();

            return result;

        }
    }
}
