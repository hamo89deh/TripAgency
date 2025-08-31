using TripAgency.Service.Feature.PackageTrip.Queries;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Feature.TypeTrip_Entity.Queries;

namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Queries
{
    public class GetPackageTripDestinationsActivitiesDatesDto
    {
        public int PackageTripId { get; set; }
        public IEnumerable<PackageTripDestinationsActivitiesDto> DestinationsActivitiesDtos { get; set; }
        public IEnumerable<PackageTripDatesDto> PackageTripDatesDtos { get; set; }

    }
}
