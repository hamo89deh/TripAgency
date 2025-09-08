using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;

namespace TripAgency.Service.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationService> _logger;

        public IBookingTripRepositoryAsync _bookingTripRepositoryAsync { get; }
        public INotificationRepositoryAsync _notificationRepositoryAsync { get; }

        public NotificationService(IBookingTripRepositoryAsync bookingTripRepositoryAsync,
                                    INotificationRepositoryAsync notificationRepositoryAsync,
                                      IEmailService emailService,
                                      ILogger<NotificationService> logger
                                  )
        {
            _bookingTripRepositoryAsync = bookingTripRepositoryAsync;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _emailService = emailService;
            _logger = logger;
        }


        public async Task<Result> CreateInAppNotificationAsync(int userId, string title, string message, string type, string? relatedEntityId = null)
        {

            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                NotificationType = type,
                Channel = NotificationChannelEnum.InApp,
                Status = NotificationStatusEnum.Pending, // يمكن أن تصبح Pending ثم Sent بواسطة Background Job
                RelatedEntityId = relatedEntityId,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now
            };
            await _notificationRepositoryAsync.AddAsync(notification);
            _logger.LogInformation("NotificationService: تم إنشاء إشعار داخل التطبيق للحجز {RelatedEntityId} للمستخدم {UserId}.", relatedEntityId, userId);
            return Result.Success();

        }

        public async Task<Result<IEnumerable<Notification>>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
        {
            var query = _notificationRepositoryAsync.GetTableNoTracking().Where(n => n.UserId == userId);
            if (unreadOnly)
            {
                query = query.Where(n => n.Status != NotificationStatusEnum.Read);
            }
            var notifications = await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
            return Result<IEnumerable<Notification>>.Success(notifications);
        }

        public async Task<Result> MarkNotificationAsReadAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepositoryAsync.GetByIdAsync(notificationId);
            if (notification == null) return Result.NotFound("الإشعار غير موجود.");
            if (notification.UserId != userId) return Result.Failure("ليس لديك صلاحية لتحديث هذا الإشعار.", failureType: ResultFailureType.Forbidden);

            if (notification.Status != NotificationStatusEnum.Read)
            {
                notification.Status = NotificationStatusEnum.Read;
                notification.ReadAt = DateTime.Now;
                notification.UpdateAt = DateTime.Now;
                await _notificationRepositoryAsync.UpdateAsync(notification);
                _logger.LogInformation("NotificationService: تم تعليم الإشعار {NotificationId} كمقروء للمستخدم {UserId}.", notificationId, userId);
            }
            return Result.Success();
        }
    }
}


