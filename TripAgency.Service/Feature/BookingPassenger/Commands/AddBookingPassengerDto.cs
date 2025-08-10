namespace TripAgency.Service.Feature.BookingPassenger.Commands
{
    public class AddBookingPassengersDto
    {
        public int BookingTripId { get; set; }
        public IEnumerable<BookingPassengerDto> BookingPassengersDto { get; set; }
    }

}
