using System.ComponentModel.DataAnnotations;

namespace TripAgency.Service.Feature.User.Command
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
    }
}
