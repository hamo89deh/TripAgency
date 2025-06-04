namespace TripAgency.Data.Entities
{
    public class PackageTripDestinationActivity
    {
        public int Id { get; set; }
        public int PackageTripDestinationId { get; set; }
        public int ActivityId { get; set; }
        public decimal Price { get; set; }

        public PackageTripDestination PackageTripDestination { get; set; }
        public Activity Activity { get; set; }


    }

}
