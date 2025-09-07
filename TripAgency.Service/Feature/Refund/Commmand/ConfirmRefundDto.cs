namespace TripAgency.Service.Feature.Refund.Commmand
{
    public class ConfirmRefundDto
    {
        public int Id { get; set; }
        public bool IsConfirm { get; set; }
        public string? TransactionRefunded { get; set; }
        public string? AdminNotes { get; set; }
    }
}
