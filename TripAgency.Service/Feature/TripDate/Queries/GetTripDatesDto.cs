using TripAgency.Data;

namespace TripAgency.Service.Feature.TripDate.Queries
{
    public class GetTripDatesDto
    {
        public int Id { get; set; }
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public int AvailableSeats { get; set; }
        public TripDataStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }
        public bool IsAvailable { get; set; }

        // public int VehicleId { get; set; }
        public int PackageTripId { get; set; }
    }
}
