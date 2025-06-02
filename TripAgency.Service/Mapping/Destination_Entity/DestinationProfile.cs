using AutoMapper;

namespace TripAgency.Service.Mapping.Destination_Entity
{
    public partial class DestinationProfile : Profile
    {
        public DestinationProfile()
        {
            EditDestinationMapping();
            AddDestinationMapping();
            GetDestinationByIdMapping();
            GetDestinationsMapping();
            GetDestinationByCityNameMapping();
        }
    }
}
