using TripAgency.Data.Entities;
using TripAgency.Feature.Destination.Queries;

namespace TripAgency.Mapping.Destination_Entity
{
    public partial class DestinationProfile
    {
        public void GetDestinationsMapping()
        {
            CreateMap<Destination, GetDestinationsDto>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.Name, op => op.MapFrom(s => s.Name))
                .ForMember(d => d.Description, op => op.MapFrom(s => s.Description))
                .ForMember(d => d.Location, op => op.MapFrom(s => s.Location))
                .ForMember(d => d.CityId, op => op.MapFrom(s => s.CityId));
        }
    }
}
