using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Commands;

namespace TripAgency.Service.Mapping.PackageTripDestinationActivity_Entity
{
    public partial class PackageTripDestinationActivityProfile
    {
        public void UpdatePackageTripDestinationActivityMapping()
        {
            CreateMap<UpdatePackageTripDestinationActivity, PackageTripDestinationActivity>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.PackageTripDestinationId, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.ActivityId, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.Price, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.StartTime, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.EndTime, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.OrderActivity, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.Duration, op => op.MapFrom(s => s.PackageTripDestinationId))
                .ForMember(d => d.Description, op => op.MapFrom(s => s.PackageTripDestinationId));
        }
    }
}
