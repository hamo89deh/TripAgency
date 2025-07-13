using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Entities;
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

        public NotificationService( IBookingTripRepositoryAsync bookingTripRepositoryAsync,
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

        public async Task NotifyTripCancellation(int packageTripDateId)
        {
            var bookings = await _bookingTripRepositoryAsync.GetTableNoTracking()
                                                            .Where(pt => pt.PackageTripDateId == packageTripDateId)
                                                            .Include(bt=>bt.PackageTripDate)
                                                            .ThenInclude(td=>td.PackageTrip)
                                                            .ToListAsync();

            foreach (var booking in bookings)
            {
                // Add to notifications table
                await _notificationRepositoryAsync.AddAsync(new Notification
                {
                    UserId = booking.UserId,
                    PackageTripDateId = packageTripDateId,
                    Title = "تم إلغاء الرحلة",
                    Message = $"تم إلغاء الرحلة {booking.PackageTripDate.PackageTrip.Name} المقررة في {booking.PackageTripDate.StartPackageTripDate:yyyy-MM-dd}",
                    CreatedAt = DateTime.UtcNow
                });

                // Send email
                await _emailService.SendEmailAsync(
                    booking.User.Email,
                    $"نأسف لإعلامك بإلغاء الرحلة {booking.PackageTripDate.PackageTrip.Name}. سيتم استرداد المبلغ خلال 3-5 أيام عمل.",
                     "إلغاء الرحلة");
            }

            _logger.LogInformation($"Sent cancellation notifications for trip {bookings[0].PackageTripDate.PackageTrip.Id}");
        }

        public async Task NotifyTripCompletion(int packageTripDateId)
        {
            var bookings = await _bookingTripRepositoryAsync.GetTableNoTracking()
                                                            .Where(pt => pt.PackageTripDateId == packageTripDateId)
                                                            .Include(bt => bt.PackageTripDate)
                                                            .ThenInclude(td => td.PackageTrip)
                                                            .ToListAsync();

            foreach (var booking in bookings)
            {
                await _notificationRepositoryAsync.AddAsync(new Notification
                {
                    UserId = booking.UserId,
                    PackageTripDateId = packageTripDateId,
                    Title = "انتهت رحلتك",
                    Message = $"نأمل أن تكون قد استمتعت برحلتك إلى {booking.PackageTripDate.PackageTrip.Name}. نرحب بتقييمك للرحلة!",
                    CreatedAt = DateTime.UtcNow
                });
            }

            _logger.LogInformation($"Sent completion notifications for trip {bookings[0].PackageTripDate.PackageTrip.Id}");
        }
    }
}

