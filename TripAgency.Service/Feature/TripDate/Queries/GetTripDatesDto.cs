using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.TripDate.Queries
{
    public class GetPackageTripDatesDto
    {
        public int Id { get; set; }
        public DateTime StartPackageTripDate { get; set; }
        public DateTime EndPackageTripDate { get; set; }
        public int AvailableSeats { get; set; }
        public PackageTripDataStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }
        public bool IsAvailable { get; set; }

        // public int VehicleId { get; set; }
        public int PackageTripId { get; set; }
    }
}
