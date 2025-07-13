using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data;

namespace TripAgency.Service.Feature.TripDate.Commands
{
    public class AddPackageTripDateDto
    {
        public DateTime StartPackageTripDate { get; set; }
        public DateTime EndPackageTripDate { get; set; } 
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }
        public int PackageTripId { get; set; }
    }
}
