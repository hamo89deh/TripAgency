using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.User.Command
{
    public class AddUserDto
    {
        public string UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
