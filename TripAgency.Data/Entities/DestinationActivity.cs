namespace TripAgency.Data.Entities
{
    public class DestinationActivity
    {
        public int Id { get; set; }
        public int DestinationId { get; set; }
        public int ActivityId { get; set; }
        public Destination Destination { get; set; }
        public Activity Activity { get; set; }

        public IEnumerable<PackageTripDestinationActivity> PackageTripDestinationActivities { get; set; }

    }

}
