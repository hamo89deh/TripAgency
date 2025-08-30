using Microsoft.AspNetCore.Http;

namespace TripAgency.Service.Feature.PackageTrip.Commands
{
    public class UpdatePackageTripDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public int Duration { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public decimal Price { get; set; }
        public int TripId { get; set; }
        public IFormFile Image { get; set; }

    }
}
