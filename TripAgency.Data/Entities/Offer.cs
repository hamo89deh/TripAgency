using System;

namespace TripAgency.Data.Entities
{
    public class Offer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal DiscountPercentage { get; set; } // نسبة الخصم (مثل 10% = 10.0)
        public DateTime StartDate { get; set; } // تاريخ بدء العرض
        public DateTime EndDate { get; set; } // تاريخ انتهاء العرض
        public bool IsActive { get; set; } // حالة العرض (مفعل/غير مفعل)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public IEnumerable<BookingTrip> BookingTrips { get; set; }
        public IEnumerable<PackageTripOffers> PackageTripOffers { get; set; }
    }

}
