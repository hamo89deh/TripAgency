using TripAgency.Data.Entities;
using TripAgency.Service.Feature.City.Command;

namespace TripAgency.Service.Mapping.City_Entity
{
    public partial class CityProfile
    {
        public void EditCityMapping()
        {
            CreateMap<UpdateCityDto, City>()
                .ForMember(d => d.Id, op => op.MapFrom(s=>s.Id))
                .ForMember(d => d.Name, op => op.MapFrom(s => s.Name));
        }
    }
}
