using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Enums;

namespace TripAgency.Data.Entities
{
    public class PaymentDiscrepancyReport
    {
        public int Id { get; set; }
        public int? PaymentId { get; set; } // معرف الحجز الذي أبلغ عنه العميل (FK)
        public int UserId { get; set; } // معرف المستخدم الذي قدم البلاغ (FK)

        public string ReportedTransactionRef { get; set; } = string.Empty; // رقم العملية المرجعي الذي أبلغ عنه العميل
        public DateTime ReportedPaymentDateTime { get; set; } // تاريخ الدفع المبلغ عنه
        public decimal ReportedPaidAmount { get; set; } // المبلغ المبلغ عنه

        public string? CustomerNotes { get; set; } // ملاحظات العميل (اختياري)
        public DateTime ReportDate { get; set; } // تاريخ تقديم البلاغ
        public PaymentDiscrepancyStatusEnum Status { get; set; } // حالة البلاغ (من Enum PaymentDiscrepancyStatusEnum)

        public string? AdminNotes { get; set; } // ملاحظات المسؤول بعد المراجعة (nullable)
        public int? ReviewedByUserId { get; set; } // معرف المسؤول الذي راجع البلاغ (FK, nullable)
        public DateTime? ReviewDate { get; set; } // تاريخ مراجعة المسؤول (nullable)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // تاريخ إنشاء السجل
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // تاريخ آخر تحديث للسجل


        // Navigation properties (علاقات EF Core)
        public Payment Payment  { get; set; } = null!;
        public User User { get; set; } = null!;
        public User? ReviewedByUser { get; set; } // المسؤول الذي راجع البلاغ (يمكن أن يكون null)

    }
}
