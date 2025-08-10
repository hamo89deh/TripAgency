using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Data.Entities
{
    public class BookingPassenger
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public int BookingTripId {  get; set; }
        public BookingTrip BookingTrip { get; set; }
    }
}
