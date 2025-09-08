namespace TripAgency.Service.Feature.Payment
{
    public class ManualPaymentDetailsDto
    {
        public int BookingId { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public decimal PaidAmount { get; set; }
        public int PaymentMethodId { get; set; } 
        public string? PaymentMethodName { get; set; } 
        public DateTime PaymentDateTime { get; set; }
        public string? CustomerNotes { get; set; }
    }
    public class SubmitManualPaymentDetailsDto
    {
        public int BookingId { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public decimal PaidAmount { get; set; }
        public int PaymentMethodId { get; set; }
        public DateTime PaymentDateTime { get; set; }
        public string? CustomerNotes { get; set; }
    }
}
