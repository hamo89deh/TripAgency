using System.ComponentModel.DataAnnotations;

namespace TripAgency.Service.Feature.User.Command
{
    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
    }
}
