namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Queries
{
    public class GetPackageTripDestinationActivitiesDto
    {
        public int PackageTripDestinationId { get; set; }
        public IEnumerable<PackageTripDestinationActivitiesDto> ActivitiesDtos { get; set; }

    }
}
