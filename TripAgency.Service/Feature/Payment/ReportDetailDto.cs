using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.Payment
{
    public class ReportDetailDto
    {
        // معلومات عن بلاغ التضارب (PaymentDiscrepancyReport) المرتبط
        public int? ReportId { get; set; }
        public PaymentDiscrepancyStatusEnum? ReportStatus { get; set; } // حالة البلاغ
        public decimal? ReportedAmount { get; set; } // المبلغ المبلغ عنه في البلاغ
        public DateTime? ReportedPaymentDateTime { get; set; } // تاريخ الدفع المبلغ عنه في البلاغ
        public string? CustomerNotes { get; set; } // ملاحظات العميل في البلاغ
        public string? AdminNotesOnReport { get; set; } // ملاحظات المسؤول على البلاغ
    }
}
