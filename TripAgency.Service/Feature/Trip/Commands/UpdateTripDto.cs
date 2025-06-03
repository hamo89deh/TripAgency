namespace TripAgency.Service.Feature.Trip.Commands
{
    public class UpdateTripDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TypeTripId { get; set; }

    }
}
