using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.BookingTrip.Queries
{
    public class GetBookingTripByIdDto
    {
        public int Id { get; set; }
        public int PassengerCount { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public decimal ActualPrice { get; set; }
        public string Notes { get; set; }
        public int TripDateId { get; set; }
        public int UserId { get; set; }

    }
}
