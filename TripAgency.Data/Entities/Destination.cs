using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Data.Entities
{
    public class Destination
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
    }
}
