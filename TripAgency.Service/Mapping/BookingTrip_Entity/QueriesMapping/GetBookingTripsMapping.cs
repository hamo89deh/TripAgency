using TripAgency.Data.Entities;
using TripAgency.Service.Feature.BookingTrip.Queries;

namespace TripAgency.Service.Mapping.BookingTrip_Entity
{
    public partial class BookingTripProfile
    {
        public void GetBookingTripsMapping()
        {
            CreateMap<BookingTrip, GetBookingTripsDto>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.TripDateId, op => op.MapFrom(s => s.PackageTripDateId))
                .ForMember(d => d.UserId, op => op.MapFrom(s => s.UserId))
                .ForMember(d => d.PassengerCount, op => op.MapFrom(s => s.PassengerCount))
                .ForMember(d => d.ActualPrice, op => op.MapFrom(s => s.ActualPrice))
                .ForMember(d => d.Notes, op => op.MapFrom(s => s.Notes))
                .ForMember(d => d.BookingDate, op => op.MapFrom(s => s.BookingDate))
                .ForMember(d => d.BookingStatus, op => op.MapFrom(s => s.BookingStatus));

        }
    }
}
