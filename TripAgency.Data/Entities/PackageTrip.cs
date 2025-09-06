using TripAgency.Data.Entities.Identity;

namespace TripAgency.Data.Entities
{
    public class PackageTrip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Rate { get; set; } = 5;
        public Trip Trip { get; set; }
        public int TripId { get; set; }
        public IEnumerable<PackageTripType> PackageTripTypes { get; set; }
        public IEnumerable<PackageTripDestination> PackageTripDestinations { get; set; }
        public IEnumerable<PackageTripDate> PackageTripDates { get; set; }
        public IEnumerable<FavoritePackageTrip>? FavoritePackageTrips { get; set; }
        public IEnumerable<PackageTripOffers> PackageTripOffers { get; set; }



    }

}
