namespace TripAgency.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
        public int LoyaltyPoints { get; set; }
        public string Password { get; set; }
        public IEnumerable<BookingTrip> BookingTrips { get; set; }


    }
}
