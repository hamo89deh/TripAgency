using TripAgency.Data.Entities.Identity;

namespace TripAgency.Data.Entities
{
    public class UserPhobias
    {
        public int UserId { get; set; }
        public int PhobiaId { get; set; }
        public Phobia Phobia { get; set; }
        public User User { get; set; }

    }

}
