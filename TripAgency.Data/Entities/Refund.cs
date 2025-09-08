using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Enums;

namespace TripAgency.Data.Entities
{
    public class Refund
    {
        public int Id { get; set; }     
        public decimal Amount { get; set; }      
        public RefundStatus Status { get; set; }

        public int? PaymentId { get; set; } // اختياري: ربط بالدفعة الأصلية إذا كانت معروفة
        public Payment Payment { get; set; } // خاصية التنقل

        public int? ReportId { get; set; } // اختياري: ربط بالبلاغ الذي أدى إلى الاسترداد
        public PaymentDiscrepancyReport Report { get; set; } // خاصية التنقل

        //public int UserId { get; set; }
        //public User User { get; set; } // خاصية التنقل

        public DateTime? RefundProcessedDate { get; set; } // تاريخ معالجة الاسترداد الفعلي
        public string? AdminNotes { get; set; } // ملاحظات المسؤول حول الاسترداد
        public string TransactionReference { get; set; } // رقم العملية الذي ارسله العميل 
        public string? TransactionRefunded { get; set; } // رقم العملية التي تم من خلالها استرجاع المبلغ 
                                                         
        public int? ProcessedByUserId { get; set; } // معرف المسؤول الذي قام بالعملية (FK)
        public User? ProcessedByUser { get; set; } // خاصية التنقل

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now; // تاريخ آخر تحديث للسجل
    }
}
