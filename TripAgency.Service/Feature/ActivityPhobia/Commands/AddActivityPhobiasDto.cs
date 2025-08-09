namespace TripAgency.Service.Feature.ActivityPhobia.Commands
{
    public class AddActivityPhobiasDto
    {
        public int ActivityId { get; set; }
        public List<int> Phobias { get; set; }

    }
}
