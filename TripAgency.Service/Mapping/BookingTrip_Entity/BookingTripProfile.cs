using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.Mapping.BookingTrip_Entity
{
    public partial class BookingTripProfile : Profile
    {
        public BookingTripProfile()
        {
            GetBookingTripsByIdMapping();
            GetBookingTripsMapping();
            AddBookingTripMapping();
            UpdateBookingTripMapping();
        }
    }
}
