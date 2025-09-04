using TripAgency.Service.Feature.DestinationActivity.Queries;

namespace TripAgency.Service.Feature.Destination.Queries
{
    public class GetDestinationsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int CityId { get; set; }
        public string ImageUrl { get; set; }
    }
    public class GetDestinationsDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<GetActivitiesDetailsDto> GetDestinationActivitiesByIds { get;set; }

    }
    public class GetActivitiesDetailsDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
