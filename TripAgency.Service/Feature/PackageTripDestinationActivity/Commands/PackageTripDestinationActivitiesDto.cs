namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Commands
{
    public class PackageTripDestinationActivitiesDto
    {
        public int ActivityId { get; set; }
        public decimal Price { get; set; }
        public int OrderActivity { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
    }
}
