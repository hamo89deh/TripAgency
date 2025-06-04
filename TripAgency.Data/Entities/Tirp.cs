using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Data.Entities
{
    public class Trip
    {
        public int Id { get; set; }  
        public string Name { get; set; }  
        public string Description { get; set; }
        public IEnumerable<PackageTrip> TripList { get; set; }

    }
}
