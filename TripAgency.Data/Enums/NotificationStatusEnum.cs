namespace TripAgency.Data.Enums
{
    public enum NotificationStatusEnum
    {
        Pending = 0, // بانتظار الإرسال/المعالجة
        Sent = 1,    // تم الإرسال/المعالجة
        Failed = 2,  // فشل الإرسال
        Read = 3     // تم القراءة (خاص بـ In-App)
    }
}
