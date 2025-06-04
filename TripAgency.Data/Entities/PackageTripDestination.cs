namespace TripAgency.Data.Entities
{
    public class PackageTripDestination
    {
        public int Id { get; set; }
        public int PackageTripId { get; set; }
        public int DestinationId { get; set; }

        public PackageTrip PackageTrip { get; set; }
        public Destination Destination { get; set; }
        public IEnumerable<PackageTripDestinationActivity> PackageTripDestinationActivities { get; set; }


    }

}
