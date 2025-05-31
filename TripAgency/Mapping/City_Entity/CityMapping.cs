using AutoMapper;
namespace TripAgency.Mapping.City_Entity
    
{
    public partial class CityProfile : Profile
    {
        public CityProfile()
        {
            GetByIdCityMapping();
            GetCitiesMapping();
            AddCityMapping();
            EditCityMapping();
        }
    }
}
