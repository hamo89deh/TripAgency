using TripAgency.Data.Enums;

namespace TripAgency.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? PaymentInstructions { get; set; }

        public int BookingTripId { get; set; }
        public string? TransactionRef { get; set; }
        public BookingTrip BookingTrip { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public int PaymentMethodId { get; set; }
    }
}
