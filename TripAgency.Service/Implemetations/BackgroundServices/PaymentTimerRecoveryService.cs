using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Enums;
using TripAgency.Infrastructure.Abstracts;
using Microsoft.Extensions.Logging;
using TripAgency.Service.Abstracts;

public class PaymentTimerRecoveryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentTimerRecoveryService> _logger;

    public PaymentTimerRecoveryService(IServiceScopeFactory scopeFactory, ILogger<PaymentTimerRecoveryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var bookingTripRepository = scope.ServiceProvider.GetRequiredService<IBookingTripRepositoryAsync>(); // استبدل YourDbContext
            var paymentTimerService = scope.ServiceProvider.GetRequiredService<IPaymentTimerService>();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

            // جلب الحجوزات التي حالتها Pending وTransactionRef فارغ أو null
            var pendingBookings = await bookingTripRepository.GetTableNoTracking()
                .Include(x=>x.Payment)
                .Where(b => b.BookingStatus == BookingStatus.Pending && string.IsNullOrEmpty( b.Payment.TransactionRef))
                .ToListAsync();

            foreach (var booking in pendingBookings)
            {
                var now = DateTime.Now;
                if (booking.ExpireTime < now)
                {
                    // المؤقت انتهى، استدعاء HandlePaymentTimeoutAsync
                    _logger.LogWarning("Booking {BookingId} has expired before restart.", booking.Id);
                    await paymentService.HandlePaymentTimeoutAsync(booking.Id);
                }
                else
                {
                    // إعادة تشغيل المؤقت للوقت المتبقي
                    var remainingTime = booking.ExpireTime - now;
                    if (remainingTime > TimeSpan.Zero)
                    {
                        _logger.LogInformation("Restarting timer for booking {BookingId} for {Seconds} seconds.", booking.Id, remainingTime.TotalSeconds);
                         paymentTimerService.StartPaymentTimer(booking.Id, remainingTime);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during timer recovery.");
        }
    }
}