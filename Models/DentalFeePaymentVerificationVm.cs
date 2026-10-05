using System;
using System.Collections.Generic;

namespace VerificationPortal.Models
{
    public class DentalFeePaymentVerificationVm
    {
        public VerificationPageContextVm PageContext { get; set; } = new();


        public List<DentalApplicableCourseVerificationVm> ApplicableCourses
        {
            get; set;
        } = new();

        // =====================================================
        // SECTION 1 : FEE STRUCTURE
        // =====================================================

        public List<DentalFeeTypeVerificationVm> FeeTypes { get; set; } = new();

        public DentalFeeSummaryVm Summary { get; set; } = new();



        // =====================================================
        // SECTION 2 : TRANSACTION RECEIPT / PAYMENT
        // =====================================================

        public DentalPaymentVerificationVm Payment { get; set; } = new();

        public List<SectionFeedbackViewModel> FeeStructureFeedback { get; set; }
            = new();


        // =====================================================
        // OPTIONAL : GENERAL PAGE FEEDBACK
        // =====================================================

        // Keep this only if your page has a general feedback section.
        public List<SectionFeedbackViewModel> SectionFeedback { get; set; }
            = new();
    }

    public class DentalApplicableCourseVerificationVm
    {
        public int CourseCode { get; set; }

        public string? CourseName { get; set; }

        public string? CourseLevel { get; set; }

        public int AcademicIntake2026 { get; set; }
    }

    // =========================================================
    // FEE TYPE
    // =========================================================

    public class DentalFeeTypeVerificationVm
    {
        public int FeeTypeId { get; set; }

        public string FeeType { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public List<DentalFeeVerificationItemVm> FeeItems { get; set; }
            = new();
    }


    // =========================================================
    // FEE ITEM
    // =========================================================

    public class DentalFeeVerificationItemVm
    {
        public int? Id { get; set; }

        public int? DentalFeeStructureId { get; set; }

        public string? CourseName { get; set; }

        public int? CourseCode { get; set; }

        public string? CourseLevel { get; set; }

        public int? AcademicIntake2026 { get; set; }

        public decimal AmountToBePaid { get; set; }

        public string? CalculationType { get; set; }

        public int Multiplier { get; set; }

        public decimal CalculatedAmount { get; set; }

        public bool IsApplicable { get; set; }
    }


    // =========================================================
    // PAYMENT / TRANSACTION RECEIPT
    // =========================================================

    public class DentalPaymentVerificationVm
    {
        public int? PaymentId { get; set; }

        public string? CourseLevel { get; set; }

        public string? TransactionId { get; set; }

        public string? TransactionReceiptPath { get; set; }
        public int? TransactionReceiptDocId { get; set; }

        public decimal AmountPaid { get; set; }

        public bool IsPaymentAvailable { get; set; }

        public bool HasTransactionReceipt =>
            !string.IsNullOrWhiteSpace(TransactionReceiptPath);
    }


    // =========================================================
    // SUMMARY
    // =========================================================

    public class DentalFeeSummaryVm
    {
        public decimal TotalFeeAmount { get; set; }

        public decimal AmountPaid { get; set; }

        public decimal BalanceAmount { get; set; }

        public string? CourseLevel { get; set; }

        public bool IsPaymentCompleted { get; set; }

        public string PaymentStatus =>
            IsPaymentCompleted
                ? "Payment Completed"
                : "Payment Pending";
    }
}