using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.TripDate.Queries
{
    public class GetPackageTripDateByIdDto
    {
        public int Id { get; set; }
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }
        public int AvailableSeats { get; set; }
        public PackageTripDateStatus Status { get; set; }
        public int PackageTripId { get; set; }
    }
}
