namespace TripAgency.Data.Enums
{
    public enum enUpdatePackageTripDataStatusDto
    {
        Published,      // منشورة ومتاحة للحجز
        BookingClosed,  // الحجز مغلق (قبل الرحلة بوقت قصير)
        Cancelled      // ملغاة (من قبل الإدارة)

    }
}
