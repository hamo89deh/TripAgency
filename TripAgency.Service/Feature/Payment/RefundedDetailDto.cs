using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.Payment
{
    public class RefundedDetailDto
    {
        // معلومات عن الاسترداد (Refund) المرتبط
        public int? RefundId { get; set; }
        public decimal? RefundAmount { get; set; }
        public RefundStatus? RefundStatus { get; set; }
        public DateTime? RefundProcessedDate { get; set; }
        public string? AdminNotesOnRefund { get; set; }

    }
}
