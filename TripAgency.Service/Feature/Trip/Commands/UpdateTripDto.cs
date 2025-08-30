using Microsoft.AspNetCore.Http;

namespace TripAgency.Service.Feature.Trip.Commands
{
    public class UpdateTripDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }

    }
}
