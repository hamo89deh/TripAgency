namespace TripAgency.Service.Feature.Trip.Queries
{
    public class GetTripByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TypeTripId { get; set; }
    }
}
