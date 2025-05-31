namespace TripAgency.Feature.Destination.Commands
{
    public class AddDestinationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int CityId { get; set; }
    }
}
