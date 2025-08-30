namespace TripAgency.Data.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<DestinationActivity> DestinationActivities { get; set; }
        public IEnumerable<PackageTripDestinationActivity> PackageTripDestinationActivities { get; set; }
        public IEnumerable<ActivityPhobias> ActivityPhobias { get; set; }

    }

}
