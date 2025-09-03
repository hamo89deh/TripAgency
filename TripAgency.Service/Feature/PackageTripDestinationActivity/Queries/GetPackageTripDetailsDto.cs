using TripAgency.Service.Feature.PromotionDto;

namespace TripAgency.Service.Feature.PackageTripDestinationActivity.Queries
{
    public class GetPackageTripDetailsDto
    {
        public int PackageTripId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int MaxCapacity { get; set; }
        public int MinCapacity { get; set; }
        public int Rating { get; set; }
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
