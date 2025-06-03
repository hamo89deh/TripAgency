using TripAgency.Data.Entities;
using TripAgency.Service.Feature.TypeTrip_Entity.Queries;

namespace TripAgency.Service.Mapping.TypTrip_Entity
{
    public partial class TypeTripProfile
    {
        public void GetTypeTripByIdMapping()
        {
             CreateMap<TypeTrip, GetTypeTripByIdDto>()
                .ForMember(s => s.Id, op => op.MapFrom(s => s.Id))
                .ForMember(s => s.Name, op => op.MapFrom(s => s.Name));
        }
    }
}
