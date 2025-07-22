using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TripAgency.Data.Enums;

namespace TripAgency.Data.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; } // المستخدم المستهدف للإشعار

        public string Title { get; set; } = string.Empty; // عنوان الإشعار
        public string Message { get; set; } = string.Empty; // محتوى الإشعار
        public string NotificationType { get; set; } = string.Empty; // نوع الإشعار (مثلاً: "BookingRejected", "BookingConfirmed")

        public NotificationChannelEnum Channel { get; set; } // قناة الإرسال (من Enum NotificationChannelEnum)
        public NotificationStatusEnum Status { get; set; }  // حالة الإشعار (من Enum NotificationStatusEnum)

        public string? RelatedEntityId { get; set; }  // معرف الكيان المرتبط (مثلاً BookingId)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // تاريخ إنشاء الإشعار
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow; // تاريخ تعديل الإشعار
        public DateTime? SentAt { get; set; } // تاريخ الإرسال الفعلي (nullable)
        public DateTime? ReadAt { get; set; } // تاريخ قراءة الإشعار (nullable، خاص بـ In-App)

        // Navigation Property
        public User User { get; set; } = null!;

    }
}
