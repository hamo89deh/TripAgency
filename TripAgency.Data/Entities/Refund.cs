using TripAgency.Data.Enums;

namespace TripAgency.Data.Entities
{
    public class Refund
    {
        public int Id { get; set; }
        public int BookingTripId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public RefundStatus Status { get; set; }   

        public BookingTrip BookingTrip { get; set; }

    }
}
