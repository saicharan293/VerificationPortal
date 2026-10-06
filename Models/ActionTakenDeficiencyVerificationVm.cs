using System;
using System.Collections.Generic;

namespace VerificationPortal.Models
{
    public class ActionTakenDeficiencyVerificationVm
    {
        // =====================================================
        // PAGE CONTEXT
        // =====================================================

        public VerificationPageContextVm PageContext { get; set; } = new();


        // =====================================================
        // ACTION TAKEN / DEFICIENCY DETAILS
        // =====================================================

        public List<ActionTakenDeficiencyItemVm> Deficiencies { get; set; }
            = new();


        // =====================================================
        // SECTION FEEDBACK
        // =====================================================

        public List<SectionFeedbackViewModel> SectionFeedback { get; set; }
            = new();


        public int? RelevantReportDocId { get; set; }

    }


    public class ActionTakenDeficiencyItemVm
    {
        public int ActionTakenDeficiencyReportId { get; set; }

        public string? CourseLevel { get; set; }

        public string DeficiencyPointedOut { get; set; }
            = string.Empty;

        public string ExtentRemedied { get; set; }
            = string.Empty;

        public string? RelevantReportPath { get; set; }

        public bool HasRelevantReport =>
            !string.IsNullOrWhiteSpace(RelevantReportPath);
    }
}