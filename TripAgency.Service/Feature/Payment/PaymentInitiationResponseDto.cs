namespace TripAgency.Service.Feature.Payment
{
    public class PaymentInitiationResponseDto
    {
        public string RedirectUrl { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string PaymentInstructions { get; set; } = string.Empty;
        public int BookingTripId { get; set; } 
        public DateTime ExpireTime { get; set; } 
    }
}
