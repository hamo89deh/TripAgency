namespace TripAgency.Data.Entities
{
    public class Promotion
    {
        public int Id { get; set; }
        public int PackageTripId { get; set; }
        public decimal DiscountPercentage { get; set; } // نسبة الخصم (مثل 10% = 10.0)
        public DateTime StartDate { get; set; } // تاريخ بدء العرض
        public DateTime EndDate { get; set; } // تاريخ انتهاء العرض
        public bool IsActive { get; set; } // حالة العرض (مفعل/غير مفعل)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } =false;

        // العلاقات
        public PackageTrip PackageTrip { get; set; }
    }

}
