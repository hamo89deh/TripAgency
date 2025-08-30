using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PackageTrip.Queries;

namespace TripAgency.Service.Mapping.PackageTripMapping
{
    public partial class PackageTripProfile
    {
        public void GetPackageTripsMapping()
        {
            CreateMap<PackageTrip, GetPackageTripsDto>()
                .ForMember(s => s.Name, op => op.MapFrom(s => s.Name))
                .ForMember(s => s.Description, op => op.MapFrom(s => s.Description))
                .ForMember(s => s.ImageUrl, op => op.MapFrom(s => s.ImageUrl))
                .ForMember(s => s.TripId, op => op.MapFrom(s => s.TripId))
                .ForMember(s => s.MinCapacity, op => op.MapFrom(s => s.MinCapacity))
                .ForMember(s => s.MaxCapacity, op => op.MapFrom(s => s.MaxCapacity))
                .ForMember(s => s.Duration, op => op.MapFrom(s => s.Duration))
                .ForMember(s => s.Price, op => op.MapFrom(s => s.Price));
        }
    }
}
