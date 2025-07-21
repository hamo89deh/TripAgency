namespace TripAgency.Service.Feature.Payment
{
    public class PaymentRequest
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "SAR";
        public string CustomerEmail { get; set; } = string.Empty;
        public string SuccessCallbackUrl { get; set; } = string.Empty;
        public string FailureCallbackUrl { get; set; } = string.Empty;
        public string CancelCallbackUrl { get; set; } = string.Empty;
    }
}
