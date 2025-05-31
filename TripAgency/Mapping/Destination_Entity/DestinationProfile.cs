using AutoMapper;

namespace TripAgency.Mapping.Destination_Entity
{
    public partial class DestinationProfile : Profile
    {
        public DestinationProfile()
        {
            EditDestinationMapping();
            AddDestinationMapping();
            GetDestinationByIdMapping();
            GetDestinationsMapping();
        }
    }
}
