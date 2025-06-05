namespace TripAgency.Data.Entities
{
    public class TripDate
    {
        public int Id { get; set; }
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public int AvailableSeats { get; set; }
        public TripStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }
        public bool IsAvailable  { get; set; } 

        // public int VehicleId { get; set; }
        public int PackageTripId { get; set; }
        public PackageTrip PackageTrip { get; set; }

    }

}
