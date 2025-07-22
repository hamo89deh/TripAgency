namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Queries
{
    public class GetPackageTripDestinationsActivitiesDto
    {
        public int PackageTripId { get; set; }
        public IEnumerable<PackageTripDestinationsActivitiesDto> DestinationsActivitiesDtos { get; set; }

    }
}
