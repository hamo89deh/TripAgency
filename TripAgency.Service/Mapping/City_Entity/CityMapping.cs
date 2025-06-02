using AutoMapper;
namespace TripAgency.Service.Mapping.City_Entity
    
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
