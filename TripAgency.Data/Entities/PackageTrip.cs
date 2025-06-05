namespace TripAgency.Data.Entities
{
    public class PackageTrip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CancellationPolicy { get; set; }      
        public int Duration { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public decimal Price { get; set; }

        public Trip Trip { get; set; }
        public int TripId { get; set; }
        public IEnumerable<PackageTripType> PackageTripTypes { get; set; }
        public IEnumerable<PackageTripDestination> PackageTripDestinations { get; set; }
        public IEnumerable<TripDate> TripDates { get; set; }



    }

}
