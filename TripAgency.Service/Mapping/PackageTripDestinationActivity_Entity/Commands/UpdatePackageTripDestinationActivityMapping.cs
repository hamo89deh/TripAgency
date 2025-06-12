using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Commands;

namespace TripAgency.Service.Mapping.PackageTripDestinationActivity_Entity
{
    public partial class PackageTripDestinationActivityProfile
    {
        public void UpdatePackageTripDestinationActivityMapping()
        {
            CreateMap<UpdatePackageTripDestinationActivityDto, PackageTripDestinationActivity>()
                .ForMember(d => d.Price, op => op.MapFrom(s => s.Price))
                .ForMember(d => d.StartTime, op => op.MapFrom(s => s.StartTime))
                .ForMember(d => d.EndTime, op => op.MapFrom(s => s.EndTime))
                .ForMember(d => d.OrderActivity, op => op.MapFrom(s => s.OrderActivity))
                .ForMember(d => d.Duration, op => op.MapFrom(s => s.Duration))
                .ForMember(d => d.Description, op => op.MapFrom(s => s.Description));
        }
    }
}
