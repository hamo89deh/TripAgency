using TripAgency.Data.Entities;
using TripAgency.Service.Feature.Destination.Commands;

namespace TripAgency.Service.Mapping.Destination_Entity
{
    public partial class DestinationProfile
    {
        public void AddDestinationMapping()
        {
            CreateMap<AddDestinationDto, Destination>()
                .ForMember(d => d.Id, op=>op.Ignore())
                .ForMember(d => d.Name, op => op.MapFrom(s => s.Name))
                .ForMember(d => d.Description, op => op.MapFrom(s => s.Description))
                .ForMember(d => d.Location, op => op.MapFrom(s => s.Location))
                .ForMember(d => d.CityId, op => op.MapFrom(s => s.CityId));
        }
    }
}
