using TripAgency.Data.Entities;
using TripAgency.Feature.City.Command;

namespace TripAgency.Mapping.City_Entity
{
    public partial class CityProfile
    {
        public void EditCityMapping()
        {
            CreateMap<EditCityDto, City>()
                .ForMember(d => d.Id, op => op.MapFrom(s=>s.Id))
                .ForMember(d => d.Name, op => op.MapFrom(s => s.Name));
        }
    }
}
