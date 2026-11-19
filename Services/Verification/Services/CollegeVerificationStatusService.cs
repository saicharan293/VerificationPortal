using Microsoft.EntityFrameworkCore;
using VerificationPortal.DATA;
using VerificationPortal.Models;
using VerificationPortal.Services.Verification.Interfaces;

namespace VerificationPortal.Services.Verification.Services
{
    public class CollegeVerificationStatusService : ICollegeVerificationStatusService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CollegeVerificationStatusService> _logger;
        public CollegeVerificationStatusService(ApplicationDbContext context, ILogger<CollegeVerificationStatusService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<TabSectionVerificationStatusVm>> GetTabVerificationDetailsAsync(int facultyId, string collegeCode, string? courseLevel, string? typeOfInstitution)
        {
            var tabs = await _context.MstTabs.AsNoTracking()
                .Where(t => t.FacultyId == facultyId)
                .Select(t => new
                {
                    t.TabId,
                    t.TabName,
                    Sections = t.MstSections
                        .Select(s => new
                        {
                            s.SectionId,
                            s.SectionName
                        }).ToList()
                })
                .ToListAsync();

            var verifiedTabs = await _context.TabWiseFeedbacks
                .AsNoTracking()
                .Where(e =>
                    e.FacultyId == facultyId &&
                    e.CollegeCode == collegeCode &&
                    e.CourseLevel == courseLevel)
                .ToDictionaryAsync(
                    e => e.TabId,
                    e => e.VerificationStatus
                );

            var sectionFeedbacks = await _context.SectionWiseFeedbacks
                .AsNoTracking()
                .Where(e =>
                    e.FacultyId == facultyId &&
                    e.CollegeCode == collegeCode &&
                    e.CourseLevel == courseLevel)
                .ToDictionaryAsync(e => new { e.TabId, e.SectionId }, e => e.VerificationStatus);


            var result = new List<TabSectionVerificationStatusVm>();

            foreach( var tab in tabs)
            {
                bool isTabVerified = verifiedTabs.TryGetValue(tab.TabId, out var tabStatus) && string.Equals(tabStatus, "accept", StringComparison.OrdinalIgnoreCase);

                var pendingSections = new List<string>();

                foreach( var section in tab.Sections)
                {
                    string sectionName = section.SectionName.Trim();
                    bool isUGSection = sectionName.EndsWith("UG", StringComparison.OrdinalIgnoreCase);
                    bool isPGSection = sectionName.EndsWith("pG", StringComparison.OrdinalIgnoreCase);

                    if (isUGSection && !string.Equals(courseLevel, "UG", StringComparison.OrdinalIgnoreCase)) continue;
                    if (isPGSection && !string.Equals(courseLevel, "PG", StringComparison.OrdinalIgnoreCase)) continue;

                    bool isSectionVerified = sectionFeedbacks.TryGetValue( new { tab.TabId, section.SectionId }, out var sectionStatus ) && string.Equals(sectionStatus, "Accepted", StringComparison.OrdinalIgnoreCase);

                    if (!isSectionVerified) pendingSections.Add(section.SectionName);
                }

                bool allSectionsVerified = pendingSections.Count == 0;
                bool isCompleted = isTabVerified && allSectionsVerified;

                if(tab.TabName == "Institution Details" && typeOfInstitution == "11")
                {
                    pendingSections.RemoveAll(e => e.StartsWith("Trust Details", StringComparison.OrdinalIgnoreCase));
                    allSectionsVerified = pendingSections.Count == 0;
                    isCompleted = isTabVerified && allSectionsVerified;
                }

                result.Add(new TabSectionVerificationStatusVm
                {
                    TabName = tab.TabName,
                    IsTabVerified = isTabVerified,
                    PendingSections = pendingSections,
                    IsCompleted = isCompleted
                });
            }

            return result;
        }



        public async Task<string> GetOverallVerificationStatusAsync( int facultyId, string collegeCode, string? courseLevel, string? typeOfInstitution)
        {
            // 1. Load all tabs and sections
            var tabs = await _context.MstTabs
                .AsNoTracking()
                .Where(t => t.FacultyId == facultyId)
                .Select(t => new
                {
                    t.TabId,
                    t.TabName,
                    Sections = t.MstSections
                        .Select(s => new
                        {
                            s.SectionId,
                            s.SectionName
                        })
                        .ToList()
                })
                .ToListAsync();

            // 2. Load tab feedback
            var tabFeedbacks = await _context.TabWiseFeedbacks
                .AsNoTracking()
                .Where(x =>
                    x.FacultyId == facultyId &&
                    x.CollegeCode == collegeCode &&
                    x.CourseLevel == courseLevel)
                .ToListAsync();

            // 3. Load section feedback
            var sectionFeedbacks = await _context.SectionWiseFeedbacks
                .AsNoTracking()
                .Where(x =>
                    x.FacultyId == facultyId &&
                    x.CollegeCode == collegeCode &&
                    x.CourseLevel == courseLevel)
                .ToListAsync();

            // 4. Check whether verification has started
            bool hasStarted = tabFeedbacks.Count > 0 || sectionFeedbacks.Count > 0;

            if (!hasStarted)  return "Not Started";

            // 5. Filter applicable tabs
            var applicableTabs = tabs.Where(tab =>
            {
                string tabName = tab.TabName?.Trim() ?? "";

                // Government College: Exclude Trust-related tabs
                if (string.Equals(
                        typeOfInstitution, "11",
                        StringComparison.OrdinalIgnoreCase))
                {
                    string[] excludedTrustTabs =
                    {
                        "Trust Details",
                        "Trust Member Details"
                    };

                    if (excludedTrustTabs.Contains( tabName, StringComparer.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                // UG: Exclude PG-related tabs
                if (string.Equals( courseLevel, "UG",  StringComparison.OrdinalIgnoreCase))
                {
                    string[] excludedPGTabs =
                    {
                        "PG Course Details",
                        "PG Academic Matters"
                    };

                    if (excludedPGTabs.Contains( tabName, StringComparer.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                // PG: Exclude UG-related tabs
                if (string.Equals(courseLevel, "PG", StringComparison.OrdinalIgnoreCase) &&
                    tabName.Contains( "UG", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return true;

            }).ToList();

            // 6. Identify applicable sections
            var applicableSections = new List<(int TabId, int SectionId)>();

            foreach (var tab in applicableTabs)
            {
                foreach (var section in tab.Sections)
                {
                    string sectionName = section.SectionName?.Trim() ?? "";

                    bool isUGSection = sectionName.EndsWith( "UG", StringComparison.OrdinalIgnoreCase);

                    bool isPGSection = sectionName.EndsWith( "PG", StringComparison.OrdinalIgnoreCase);

                    // Exclude UG sections for PG
                    if (isUGSection && !string.Equals( courseLevel, "UG", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // Exclude PG sections for UG
                    if (isPGSection && !string.Equals( courseLevel, "PG", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // Government College: Ignore Trust Details section
                    if (string.Equals( typeOfInstitution, "11", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals( tab.TabName, "Institution Details", StringComparison.OrdinalIgnoreCase) &&
                        sectionName.StartsWith("Trust Details", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    applicableSections.Add( (tab.TabId, section.SectionId));
                }
            }

            // 7. Check rejected tabs
            bool isTabRejected = false;

            foreach (var tab in applicableTabs)
            {
                var feedback = tabFeedbacks.FirstOrDefault( x => x.TabId == tab.TabId);

                string? status = feedback?.VerificationStatus?.Trim();

                if (string.Equals( status, "Rejected", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals( status, "Reject", StringComparison.OrdinalIgnoreCase))
                {
                    isTabRejected = true;

                    _logger.LogWarning(
                        "TAB REJECTED | CollegeCode: {CollegeCode} | FacultyId: {FacultyId} | Tab: {TabName} | Status: {Status}",
                        collegeCode,
                        facultyId,
                        tab.TabName,
                        status);
                }
            }

            // 8. Check rejected applicable sections
            bool isSectionRejected = false;

            foreach (var feedback in sectionFeedbacks)
            {
                bool isApplicable = applicableSections.Any(x =>
                    x.TabId == feedback.TabId &&
                    x.SectionId == feedback.SectionId);

                if (!isApplicable)
                    continue;

                string? status = feedback.VerificationStatus?.Trim();

                if (string.Equals( status, "Rejected", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals( status, "Reject", StringComparison.OrdinalIgnoreCase))
                {
                    isSectionRejected = true;

                    var tab = applicableTabs.FirstOrDefault( x => x.TabId == feedback.TabId);

                    var section = tab?.Sections.FirstOrDefault( x => x.SectionId == feedback.SectionId);

                    _logger.LogWarning(
                        "SECTION REJECTED | CollegeCode: {CollegeCode} | FacultyId: {FacultyId} | Tab: {TabName} | Section: {SectionName} | Status: {Status}",
                        collegeCode,
                        facultyId,
                        tab?.TabName,
                        section?.SectionName,
                        status);
                }
            }

            // Rejected takes priority
            if (isTabRejected || isSectionRejected)
                return "Rejected";

            // 9. Check pending tabs and sections
            bool hasPending = false;

            foreach (var tab in applicableTabs)
            {
                var tabFeedback = tabFeedbacks.FirstOrDefault( x => x.TabId == tab.TabId);

                string? tabStatus = tabFeedback?.VerificationStatus?.Trim();

                bool isTabVerified =
                    string.Equals( tabStatus, "accept", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals( tabStatus, "accepted", StringComparison.OrdinalIgnoreCase);

                if (!isTabVerified)
                {
                    hasPending = true;

                    _logger.LogWarning(
                        "TAB NOT VERIFIED | CollegeCode: {CollegeCode} | FacultyId: {FacultyId} | Tab: {TabName} | Status: {Status}",
                        collegeCode,
                        facultyId,
                        tab.TabName,
                        tabStatus ?? "No Feedback");
                }

                foreach (var section in tab.Sections)
                {
                    bool isApplicable = applicableSections.Any(x =>
                        x.TabId == tab.TabId &&
                        x.SectionId == section.SectionId);

                    if (!isApplicable)
                        continue;

                    var sectionFeedback = sectionFeedbacks.FirstOrDefault(
                        x =>
                            x.TabId == tab.TabId &&
                            x.SectionId == section.SectionId);

                    string? sectionStatus = sectionFeedback?.VerificationStatus?.Trim();

                    bool isSectionVerified =
                        string.Equals(
                            sectionStatus, "Accepted",
                            StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(
                            sectionStatus, "accept",
                            StringComparison.OrdinalIgnoreCase);

                    if (!isSectionVerified)
                    {
                        hasPending = true;

                        _logger.LogWarning(
                            "SECTION NOT VERIFIED | CollegeCode: {CollegeCode} | FacultyId: {FacultyId} | Tab: {TabName} | Section: {SectionName} | Status: {Status}",
                            collegeCode,
                            facultyId,
                            tab.TabName,
                            section.SectionName,
                            sectionStatus ?? "No Feedback");
                    }
                }
            }

            // 10. Final overall status
            return hasPending ? "Pending" : "Accepted";
        }
    }
}
