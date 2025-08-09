using TripAgency.Service.Feature.TypeTrip_Entity.Queries;

namespace TripAgency.Service.Feature.PackageTripType.Queries
{
    public class GetPackageTripTypesDto
    {
        public int PackageTripId { get; set; }
        public List<GetTripTypesDto> TripTypesDtos { get; set; }

    }
}
