using TripAgency.Data.Entities;
using TripAgency.Service.Feature.TypeTrip_Entity.Commands;

namespace TripAgency.Service.Mapping.TypTrip_Entity
{
    public partial class TypeTripProfile
    {
        public void UpdateTypeTripMapping()
        {
            CreateMap<UpdateTypeTripDto, TypeTrip>()
                .ForMember(s => s.Id, op => op.MapFrom(s => s.Id))
                .ForMember(s => s.Name, op => op.MapFrom(s => s.Name));

        }
    }
}
