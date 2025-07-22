using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.Payment
{
    // معلومات عن الدفعة (Payment)
    public class PaymentDetailDto
    {
        public int? PaymentId {  get; set; }
        public decimal? PaymentAmount { get; set; }
        public decimal? PaymentVerifiedAmount { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? PaymentMethodName { get; set; }
        public string? PaymentErrorDetails { get; set; }

    }
}
