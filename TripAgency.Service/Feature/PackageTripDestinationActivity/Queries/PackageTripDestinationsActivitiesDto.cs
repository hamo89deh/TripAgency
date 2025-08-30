namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Queries
{
    public class PackageTripDestinationsActivitiesDto
    {
        public int DestinationId { get; set; }
        //public int DayNumber { get; set; }
        //public int OrderDestination { get; set; }
        //public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; }
        //public string Description { get; set; }
        //public int Duration { get; set; }
        public IEnumerable<PackageTripDestinationActivitiesDto> ActivitiesDtos { get; set; }

    }
}
