namespace TripAgency.Feature.City.Queries

{
    public class GetCityByIdDto 
    {
        public int Id { get; set; }

        public string? Name { get; set; } = null;
        public GetCityByIdDto(int id)
        {
            Id = id;
        }
    }
}
