using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.TripDate.Commands
{
    public class UpdateTripDateDto
    {
        public int Id { get; set; }     
        public TripDataStatus Status { get; set; }
    }
}
