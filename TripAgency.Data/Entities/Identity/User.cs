using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripAgency.Data.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string FullName { get; set; }
        public int LoyaltyPoints { get; set; } = 0;

        public string? Code { get; set; }

        public IEnumerable<BookingTrip>? BookingTrips { get; set; } 
        public IEnumerable<FavoritePackageTrip>? FavoritePackageTrips { get; set; }
        public IEnumerable<UserPhobias>? UserPhobias { get; set; }
        public IEnumerable<TripReview> TripReviews { get; set; } 


        [InverseProperty(nameof(UserRefreshToken.User))]
        public virtual ICollection<UserRefreshToken>? UserRefreshTokens { get; set; }
    }
}
