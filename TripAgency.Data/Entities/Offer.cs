using System;

namespace TripAgency.Data.Entities
{
    public class Offer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal DiscountPercentage { get; set; } // نسبة الخصم (مثل 10% = 10.0)
        public DateOnly StartDate { get; set; } // تاريخ بدء العرض
        public DateOnly EndDate { get; set; } // تاريخ انتهاء العرض
        public bool IsActive { get; set; } // حالة العرض (مفعل/غير مفعل)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public IEnumerable<BookingTrip> BookingTrips { get; set; }
        public IEnumerable<PackageTripOffers> PackageTripOffers { get; set; }
    }

}
