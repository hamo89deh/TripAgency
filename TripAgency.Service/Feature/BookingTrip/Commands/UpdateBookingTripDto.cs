using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.BookingTrip.Commands
{
    public class UpdateBookingTripDto
    {
        public int PassengerCount { get; set; }
        public string Notes { get; set; }
    }
}
