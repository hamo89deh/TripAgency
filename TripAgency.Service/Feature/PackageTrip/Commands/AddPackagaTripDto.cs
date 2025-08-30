using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.PackageTrip.Commands
{
    public class AddPackageTripDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }

        public IFormFile Image { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public decimal Price { get; set; }       
        public int TripId { get; set; }

    }
}
