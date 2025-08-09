namespace TripAgency.Data.Entities
{
    public class Phobia
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<UserPhobias> UserPhobias { get; set; }
        public IEnumerable<ActivityPhobias> ActivityPhobias { get; set; }
    }

}
