using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.TripDate.Queries
{
    public class GetTripDateByIdDto
    {
        public int Id { get; set; }
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public int AvailableSeats { get; set; }
        public TripDataStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }
        public bool IsAvailable { get; set; }

        // public int VehicleId { get; set; }
        public int PackageTripId { get; set; }
    }
}
