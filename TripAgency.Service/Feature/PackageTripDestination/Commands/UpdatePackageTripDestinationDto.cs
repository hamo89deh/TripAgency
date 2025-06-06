namespace TripAgency.Service.Feature.PackageTripDestination.Commands
{
    public class UpdatePackageTripDestinationDto
    {
        public int Id { get; set; }
        public int TripDestinationId { get; set; }
        public int DayNumber { get; set; }
        public int OrderDestination { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
    }
}
