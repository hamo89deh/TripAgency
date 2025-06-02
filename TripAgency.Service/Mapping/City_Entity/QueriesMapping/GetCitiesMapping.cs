using TripAgency.Data.Entities;
using TripAgency.Service.Feature.City.Queries;

namespace TripAgency.Service.Mapping.City_Entity
{
    public partial class CityProfile
    {
        public void GetCitiesMapping()
        {
            CreateMap<City, GetCitiesDto>()
                .ForMember(d => d.Id, op => op.MapFrom(s => s.Id))
                .ForMember(d => d.Name, op => op.MapFrom(s => s.Name));
        }
    }
}
