using TripAgency.Data.Entities;
using TripAgency.Service.Feature.TripDate.Queries;

namespace TripAgency.Service.Mapping.TripDate_Entity
{
    public partial class TripDateProfile
    {
        public void GetTripDatesMapping()
        {
            CreateMap<PackageTripDate, GetPackageTripDatesDto>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.StartBookingDate, op => op.MapFrom(s => s.StartBookingDate))
                .ForMember(d => d.EndBookingDate, op => op.MapFrom(s => s.EndBookingDate))
                .ForMember(d => d.StartPackageTripDate, op => op.MapFrom(s => s.StartPackageTripDate))
                .ForMember(d => d.EndPackageTripDate, op => op.MapFrom(s => s.EndPackageTripDate))
                .ForMember(d => d.AvailableSeats, op => op.MapFrom(s => s.AvailableSeats))
                .ForMember(d => d.CreateDate, op => op.MapFrom(s => s.CreateDate))
                .ForMember(d => d.PackageTripId, op => op.MapFrom(s => s.PackageTripId))
                .ForMember(d => d.IsAvailable, op => op.MapFrom(s => s.IsAvailable))
                .ForMember(d => d.Status, op => op.MapFrom(s => s.Status));
        }
    }
}
