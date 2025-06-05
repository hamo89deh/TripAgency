namespace TripAgency.Data.Entities
{
    public class PackageTripDestination
    {
        public int Id { get; set; }
        public int PackageTripId { get; set; }
        public int DestinationId { get; set; }

        public int DayNumber { get; set; }
        public int OrderDestination { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; }
        public int  Duration { get; set; }
        

        public PackageTrip PackageTrip { get; set; }
        public Destination Destination { get; set; }
        public IEnumerable<PackageTripDestinationActivity> PackageTripDestinationActivities { get; set; }


    }

}
