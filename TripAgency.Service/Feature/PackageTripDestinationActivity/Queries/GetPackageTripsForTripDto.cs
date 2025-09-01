namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Queries
{
    public class GetPackageTripsForTripDto
    {
        public int TripId { get; set; }
        public IEnumerable<PackageTripForTripDto> PackageTripForTripDtos { get; set; }

    }
    public class PackageTripForTripDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public int Rating { get; set; } = 5;
        public int AvailableSeats { get; set; }
        public decimal ActulPrice { get; set; }
        public string ImageUrl { get; set; }
        public int TripId { get; set; }
        public IEnumerable<PackageTripCitiesDto> PackageTripCitiyDto { get; set; } = [];
        public IEnumerable<PackageTripHotelsDto> PackageTripHotelsDto { get; set; } = [];
        public IEnumerable<PackageTripTypesForTripDto> PackageTripTypesDtos { get; set; } = [];
        public IEnumerable<PackageTripDestinationsForTripDto> PackageTripDestinationsDtos { get; set; } = [];
    }
    public class PackageTripDatesForTripDto
    {
        public int Id { get; set; }
        public DateTime StartPackageTripDate { get; set; }
        public DateTime EndPackageTripDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }

    }
    public class PackageTripDestinationsForTripDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<PackageTripDestinationActivitiesForTripDto> packageTripDestinationActivitiesForTrips { get; set; } = [];
    }
    public class PackageTripDestinationActivitiesForTripDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class PackageTripCitiesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class PackageTripHotelsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class PackageTripTypesForTripDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
