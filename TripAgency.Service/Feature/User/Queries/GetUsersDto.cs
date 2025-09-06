namespace TripAgency.Service.Feature.User.Queries
{
    public class GetUsersDto
    {
        public int Id { get; set; }

        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; } 
        public int LoyaltyPoints { get; set; } 
        public string ImageUrl { get; set; } 
    }
}
