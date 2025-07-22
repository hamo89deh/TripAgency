namespace TripAgency.Service.Feature.Payment
{
    public class PaymentCallbackResult
    {
        public string TransactionId { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public string ErrorDetails { get; set; } = string.Empty;
    }

}
