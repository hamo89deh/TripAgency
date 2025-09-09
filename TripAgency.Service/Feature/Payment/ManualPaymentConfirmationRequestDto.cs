namespace TripAgency.Service.Feature.Payment
{
    public class ManualPaymentConfirmationRequestDto
    {
        public string TransactionRef { get; set; }

        public bool IsConfirmed { get; set; }
        public string? AdminNotes { get; set; } = string.Empty;
        public int PaymentMethodId { get; set; }
        public decimal VerifiedAmount { get; set; }
    }

}
