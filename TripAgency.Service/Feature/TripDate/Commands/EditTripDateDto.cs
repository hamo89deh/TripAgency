using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.TripDate.Commands
{
    public class UpdatePackageTripDateDto
    {
        public int Id { get; set; }     
        public PackageTripDataStatusDto Status { get; set; }
    }
}
