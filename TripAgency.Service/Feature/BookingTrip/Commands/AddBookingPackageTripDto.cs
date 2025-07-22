using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Feature.BookingTrip.Commands
{
    public class AddBookingPackageTripDto
    {
        public int PassengerCount { get; set; }
        public decimal AmountPrice { get; set; }
        public int PackageTripDateId { get; set; }
        public string Notes { get; set; }
        public int PaymentMethodId { get; set; } 
    }
}
