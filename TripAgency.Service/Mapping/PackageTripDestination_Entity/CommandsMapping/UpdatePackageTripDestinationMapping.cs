using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PackageTripDestination.Commands;

namespace TripAgency.Service.Mapping.PackageTripDestination_Entity
{
    public partial class PackageTripDestinationProfile
    {
        public void UpdatePackageTripDestinationMapping()
        {
            CreateMap<UpdatePackageTripDestinationDto, PackageTripDestination>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.PackageTripId, op => op.MapFrom(s => s.PackageTripId))
                .ForMember(d => d.DestinationId, op => op.MapFrom(s => s.DestinationId))
                .ForMember(d => d.DayNumber, op => op.MapFrom(s => s.DayNumber))
                .ForMember(d => d.StartTime, op => op.MapFrom(s => s.StartTime))
                .ForMember(d => d.EndTime, op => op.MapFrom(s => s.EndTime))
                .ForMember(d => d.OrderDestination, op => op.MapFrom(s => s.OrderDestination))
                .ForMember(d => d.Duration, op => op.MapFrom(s => s.Duration))
                .ForMember(d => d.Description, op => op.MapFrom(s => s.Description));
        }
    }
}
