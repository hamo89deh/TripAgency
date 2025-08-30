namespace TripAgency.Service.Feature.Destination.Queries
{
    public class GetDestinationsByCityNameDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string ImageUrl { get; set; }
    }
}
