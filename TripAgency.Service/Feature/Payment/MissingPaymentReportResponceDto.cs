namespace TripAgency.Service.Feature.Payment
{
    public class MissingPaymentReportResponceDto
    {
        public string TransactionReference { get; set; } = string.Empty; // رقم الـ Transaction ID من بوابة الدفع أو مرجع التحويل اليدوي
        public int ReportId { get; set; }
        public DateTime PaymentDateTime { get; set; } // الوقت الفعلي الذي تم فيه الدفع حسب العميل
        public decimal PaidAmount { get; set; } // المبلغ الذي دفعه العميل
        public string? CustomerNotes { get; set; } // ملاحظات إضافية من العميل
    }

}
