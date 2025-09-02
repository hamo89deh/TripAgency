using TripAgency.Data.Enums;

namespace TripAgency.Data.Entities
{
    public class PackageTripDate
    {
        public int Id { get; set; }
        public DateTime StartPackageTripDate { get; set; }
        public DateTime EndPackageTripDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; }
        public DateTime CreateDate { get; set; }
        public PackageTripDateStatus Status { get; set; }
        public int AvailableSeats { get; set; }

        public bool IsAvailable  { get; set; } 

        // public int VehicleId { get; set; }
        public int PackageTripId { get; set; }
        public PackageTrip PackageTrip { get; set; }
        public IEnumerable<BookingTrip> BookingTrips { get; set; }
        public IEnumerable<TripReview> TripReviews { get; set; }

    }

}
