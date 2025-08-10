using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Enums;

namespace TripAgency.Data.Entities
{
    public class BookingTrip
    {
        public int Id { get; set; }
        public int PassengerCount { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public decimal ActualPrice { get; set; }
        public string Notes { get; set; }
        public PackageTripDate PackageTripDate { get; set; }  
        public int PackageTripDateId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Payment Payment { get; set; }
        public IEnumerable<BookingPassenger> BookingPassengers { get; set; }

    }
}
