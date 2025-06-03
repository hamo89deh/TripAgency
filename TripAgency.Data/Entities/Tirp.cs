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
        public int Name { get; set; }  
        public string Description { get; set; }
        public TypeTrip TypeTrip { get; set; }  
        public int TypeTripId { get; set; }  

    }
}
