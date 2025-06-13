namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Queries
{
    public class GetPackageTripDestinationActivitiesDto
    {
        public int PackageTripId { get; set; }
        public int DestinationId { get; set; }
        public IEnumerable<PackageTripDestinationActivitiesDto> ActivitiesDtos { get; set; }

    }
}
