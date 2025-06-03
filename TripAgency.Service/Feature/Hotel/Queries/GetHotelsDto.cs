namespace TripAgency.Service.Feature.Hotel.Queries
{
    public class GetHotelsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public int Rate { get; set; }
        public int MidPriceForOneNight { get; set; }
        public int CityId { get; set; }
    }
}
