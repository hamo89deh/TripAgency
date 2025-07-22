using TripAgency.Data.Entities;
using TripAgency.Service.Feature.BookingTrip.Commands;

namespace TripAgency.Service.Mapping.BookingTrip_Entity
{
    public partial class BookingTripProfile
    {
        public void UpdateBookingTripMapping()
        {
            CreateMap<UpdateBookingTripDto, BookingTrip>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.PassengerCount, op => op.MapFrom(s => s.PassengerCount))
                .ForMember(d => d.Notes, op => op.MapFrom(s => s.Notes));


        }
    }
}
