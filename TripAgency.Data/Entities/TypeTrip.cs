namespace TripAgency.Data.Entities
{
    public class TypeTrip
    {
        public int Id { get; set; }
        public int Name { get; set; }

        public IEnumerable<Trip> Trip { get; set;}
    }
}
