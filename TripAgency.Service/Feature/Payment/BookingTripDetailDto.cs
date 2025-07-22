using TripAgency.Data.Enums;

namespace TripAgency.Service.Feature.Payment
{
    public class BookingTripDetailDto
    {
        // معلومات عن الحجز (BookingTrip) المرتبط
        public int? AssociatedBookingId { get; set; }
        public BookingStatus? BookingStatus { get; set; } // حالة الحجز المرتبط
        public decimal? BookingActualPrice { get; set; } // السعر المطلوب للحجز
        public decimal? BookingTotalPaidAmount { get; set; } // إجمالي المدفوع للحجز
        public DateTime? TripDate { get; set; } // تاريخ بداية الرحلة
        public PackageTripDateStatus? TripDateStatus { get; set; } // حالة تاريخ الرحلة
    }
}
