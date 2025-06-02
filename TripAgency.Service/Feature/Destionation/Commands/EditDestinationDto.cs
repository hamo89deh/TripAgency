namespace TripAgency.Service.Feature.Destination.Commands
{
    public class UpdateDestinationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int CityId { get; set; }
    }
}
