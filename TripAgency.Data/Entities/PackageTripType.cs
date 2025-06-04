namespace TripAgency.Data.Entities
{
    public class PackageTripType
    {
        public int Id { get; set; }
        public int PackageTripId { get; set; }
        public int TypeTripId { get; set; }

        public PackageTrip PackageTrip { get; set;}
        public TypeTrip TypeTrip { get; set;}
    }

}
