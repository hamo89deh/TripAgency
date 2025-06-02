using TripAgency.Data.Entities;
using TripAgency.Service.Feature.City.Command;

namespace TripAgency.Service.Mapping.City_Entity
{
    public partial class CityProfile
    {
        public void AddCityMapping()
        { 
            CreateMap<AddCityDto, City>()
                .ForMember(d=>d.Id, op=>op.Ignore())
                .ForMember(d=>d.Name , op=>op.MapFrom(s=>s.Name));
        }
    }
}
