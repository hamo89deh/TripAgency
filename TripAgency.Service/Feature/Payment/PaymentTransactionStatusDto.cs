namespace TripAgency.Service.Feature.Payment
{
    public class PaymentTransactionStatusDto
    {
        public string TransactionReference { get; set; } = string.Empty; // رقم العملية الذي تم البحث عنه

        public bool ExistsInSystem { get; set; } // هل يوجد سجل دفعة بهذا الرقم؟

        public PaymentDetailDto? PaymentDetailDto { get; set; }
        public BookingTripDetailDto? BookingTripDetailDto { get; set; }
        public RefundedDetailDto? RefundedDetailDto { get; set; }
        public ReportDetailDto? ReportDetailDto { get; set; }
        // تصنيف عام لنوع التضارب
        public string? DiscrepancyType { get; set; } // مثال: "مفقودة", "زائدة", "مكررة", "مستردة"
    }
}
