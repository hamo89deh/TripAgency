namespace TripAgency.Service.Feature.Activity.Queries
{
    public class GetActivityByIdDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

    }
}
