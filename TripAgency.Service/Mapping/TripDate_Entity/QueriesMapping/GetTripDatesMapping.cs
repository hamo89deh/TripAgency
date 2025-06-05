using TripAgency.Data.Entities;
using TripAgency.Service.Feature.TripDate.Queries;

namespace TripAgency.Service.Mapping.TripDate_Entity
{
    public partial class TripDateProfile
    {
        public void GetTripDatesMapping()
        {
            CreateMap<TripDate, GetTripDateByIdDto>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.StartBookingDate, op => op.MapFrom(s => s.StartBookingDate))
                .ForMember(d => d.EndBookingDate, op => op.MapFrom(s => s.EndBookingDate))
                .ForMember(d => d.StartTripDate, op => op.MapFrom(s => s.StartTripDate))
                .ForMember(d => d.EndTripDate, op => op.MapFrom(s => s.EndTripDate))
                .ForMember(d => d.AvailableSeats, op => op.MapFrom(s => s.AvailableSeats))
                .ForMember(d => d.CreateDate, op => op.MapFrom(s => s.CreateDate))
                .ForMember(d => d.PackageTripId, op => op.MapFrom(s => s.PackageTripId))
                .ForMember(d => d.IsAvailable, op => op.MapFrom(s => s.IsAvailable))
                .ForMember(d => d.Status, op => op.MapFrom(s => s.Status));
        }
    }
}
