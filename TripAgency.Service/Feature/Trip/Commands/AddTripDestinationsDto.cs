namespace TripAgency.Service.Feature.Trip.Commands
{
    public class AddTripDestinationsDto
    {
        public int TripId { get; set; }
        public IEnumerable<AddTripDestinationIdDto> DestinationIdDto { get; set; }

    } 
}
