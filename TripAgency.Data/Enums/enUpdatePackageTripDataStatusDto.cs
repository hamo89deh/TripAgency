namespace TripAgency.Data.Enums
{
    public enum enUpdatePackageTripDataStatusDto
    {
        Published     = 0,    // منشورة ومتاحة للحجز
        BookingClosed = 1,    // الحجز مغلق (قبل الرحلة بوقت قصير)
        Cancelled     = 2     // ملغاة (من قبل الإدارة)

    }
}
