using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public TripDate TripDate { get; set; }  
        public int TripDateId { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public Payment Payment { get; set; }

    }
}
