using TripAgency.Service.Feature.Destination.Queries;

namespace TripAgency.Service.Feature.Trip.Queries
{
    public class GetTripDestinationsDto
    {
        public int TripId { get; set; }
        public IEnumerable<GetDestinationByIdDto> DestinationsDto { get; set; }

    }
}
