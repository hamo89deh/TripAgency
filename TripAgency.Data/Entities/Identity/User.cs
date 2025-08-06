using Microsoft.AspNetCore.Identity;

namespace TripAgency.Data.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string FullName { get; set; }
        public int LoyaltyPoints { get; set; } = 0;
        public IEnumerable<BookingTrip>? BookingTrips { get; set; }


    }
}
