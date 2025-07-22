using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.TripDate.Queries
{
    public class PackageTripDatesDto
    {
        public int Id { get; set; }
        public DateTime StartPackageTripDate { get; set; }
        public DateTime EndPackageTripDate { get; set; }
        public DateTime StartBookingDate { get; set; }
        public DateTime EndBookingDate { get; set; } 

        public enPackageTripDataStatusDto Status { get; set; } 
    }
}
