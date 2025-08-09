namespace TripAgency.Data.Entities
{
    public class ActivityPhobias
    {
        public int ActivityId { get; set; }
        public int PhobiaId { get; set; }
        public Phobia Phobia { get; set; }
        public Activity Activity { get; set; }

    }

}
