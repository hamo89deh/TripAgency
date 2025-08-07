using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.Favorite
{
    public class GetFavoritePackageTripsDto
    {
        public int PackageTripId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    } 
}
