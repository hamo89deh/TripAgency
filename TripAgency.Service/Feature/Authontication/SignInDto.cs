using System.ComponentModel.DataAnnotations;

namespace TripAgency.Service.Feature.Authontication
{
    public class SignInDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
