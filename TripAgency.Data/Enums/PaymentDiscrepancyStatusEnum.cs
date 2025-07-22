namespace TripAgency.Data.Enums
{
    public enum PaymentDiscrepancyStatusEnum
    {
        PendingReview = 0,     // بانتظار مراجعة المسؤول
        ReviewedConfirmed = 1, // تم المراجعة والتأكيد 
        ReviewedRejected = 2,  // تم المراجعة والرفض (العميل لم يدفع أو تم استرداد المبلغ)
        Closed = 3             // تم إغلاق البلاغ (بعد اتخاذ الإجراءات اللازمة، أو لم يعد بحاجة لمتابعة)
    }
}
