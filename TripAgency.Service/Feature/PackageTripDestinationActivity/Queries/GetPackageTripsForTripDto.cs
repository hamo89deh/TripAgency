using TripAgency.Service.Feature.PromotionDto;

namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Queries
{
    public class GetPackageTripsForTripDto
    {
        public int TripId { get; set; }
        public IEnumerable<PackageTripForTripDto> PackageTripForTripDtos { get; set; }

    }
    public class PackageTripForTripDto
    {
        public int PackageTripId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public decimal Rating { get; set; } = 5;
        public decimal ActulPrice { get; set; }
        public decimal? PriceAfterPromotion { get; set; }
        public string ImageUrl { get; set; }
        public int TripId { get; set; }
        public GetPromotionByIdDto? GetPromotionByIdDto { get; set; }     
        public IEnumerable<PackageTripCitiesDto> PackageTripCitiyDto { get; set; } = [];
        public IEnumerable<PackageTripTypesForTripDto> PackageTripTypesDtos { get; set; } = [];
    }
    public class PackageTripDatesForTripDto
    {
        public int Id { get; set; }
        public DateTime StartPackageTripDate { get; set; }
        public DateTime EndPackageTripDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }
        public int AvailableSeats { get; set; }


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
    public class GetPackageTripDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public decimal Rating { get; set; }
        public decimal ActualPrice { get; set; }
        public decimal? PriceAfterPromotion { get; set; }
        public string ImageUrl { get; set; }
        public int TripId { get; set; }
        public GetPromotionByIdDto? GetPromotionByIdDto { get; set; }
        public IEnumerable<PackageTripCitiesDto> PackageTripCitiesDto { get; set; } = [];
        public IEnumerable<PackageTripHotelsDto> PackageTripHotelsDto { get; set; } = [];
        public IEnumerable<PackageTripTypesForTripDto> PackageTripTypesDtos { get; set; } = [];
        public IEnumerable<PackageTripDestinationsForTripDto> PackageTripDestinationsDtos { get; set; } = [];
        public IEnumerable<PackageTripDatesForTripDto> PackageTripDates { get; set; } = [];
    }


}
