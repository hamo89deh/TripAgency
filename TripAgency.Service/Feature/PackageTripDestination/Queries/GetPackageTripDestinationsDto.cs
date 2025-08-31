namespace TripAgency.Service.Feature.PackageTripDestination.Queries
{
    public class GetPackageTripDestinationsDto
    {    
        public int PackageTripId { get; set; }
        public IEnumerable<PackageTripDestinationDto> DestinationDto { get; set; } 
    }

}
