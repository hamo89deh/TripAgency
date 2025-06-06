using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data;

namespace TripAgency.Service.Feature.TripDate.Commands
{
    public class AddTripDateDto
    {
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }

        public int AvailableSeats {  get; set; }
        // public int VehicleId { get; set; }
        public int PackageTripId { get; set; }
    }
}
