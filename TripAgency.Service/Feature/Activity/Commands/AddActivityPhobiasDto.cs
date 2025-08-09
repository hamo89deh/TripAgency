namespace TripAgency.Service.Feature.Activity.Commands
{
    public class AddActivityPhobiasDto
    {
        public int ActivityId { get; set; }
        public List<int> Phobias { get; set; }

    }
}
