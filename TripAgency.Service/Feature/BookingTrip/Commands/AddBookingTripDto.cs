using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.BookingTrip.Commands
{
    public class AddBookingTripDto
    {
        public int Id { get; set; }
        public int PassengerCount { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public decimal ActualPrice { get; set; }
        public string Notes { get; set; }
        public int TripDateId { get; set; }
        public int UserId { get; set; }

    }
}
