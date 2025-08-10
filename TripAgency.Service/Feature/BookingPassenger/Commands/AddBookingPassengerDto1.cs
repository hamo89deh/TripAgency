namespace TripAgency.Service.Feature.BookingPassenger.Commands
{
    public class AddBookingPassengerDto
    {
        public int BookingTripId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }

    }
}
