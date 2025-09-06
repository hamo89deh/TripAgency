using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TripAgency.Infrastructure.Abstracts;
using Microsoft.Extensions.Logging;

public class OfferAndRatingUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OfferAndRatingUpdateService> _logger;
    private int DefaultRating = 5;
    private int MinimumReviews = 5;


    public OfferAndRatingUpdateService(IServiceProvider serviceProvider, ILogger<OfferAndRatingUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting PromotionAndRatingUpdateService at {Time}", DateTime.UtcNow);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var offerRepository = scope.ServiceProvider.GetRequiredService<IOffersRepositoryAsync>();
                    var tripReviewRepository = scope.ServiceProvider.GetRequiredService<ITripReviewRepositoryAsync>();
                    var packageTripRepository = scope.ServiceProvider.GetRequiredService<IPackageTripRepositoryAsync>();

                    // تحديث العروض الترويجية
                    var updatedoffersCount = await offerRepository.GetTableNoTracking()
                        .Where(p => p.IsActive && p.EndDate < DateTime.Now)
                        .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsActive, false), stoppingToken);

                    _logger.LogInformation("Updated {Count} expired promotions.", updatedoffersCount);

                    // تحديث التقييمات
                    var packageTrips = await packageTripRepository.GetTableNoTracking()
                        .Select(pt => new { pt.Id })
                        .ToListAsync(stoppingToken);

                    foreach (var packageTrip in packageTrips)
                    {
                        var validReviewsCount = await tripReviewRepository.GetTableNoTracking()
                            .Include(p => p.PackageTripDate)
                            .Where(r => r.PackageTripDate.PackageTripId == packageTrip.Id && r.Rating >= 1 && r.Rating <= 5)
                            .CountAsync(stoppingToken);

                        int rating = DefaultRating; // القيمة الافتراضية (مثل 5)
                        if (validReviewsCount >= MinimumReviews)
                        {
                            var averageRating = await tripReviewRepository.GetTableNoTracking()
                                .Include(p => p.PackageTripDate)
                                .Where(r => r.PackageTripDate.PackageTripId == packageTrip.Id && r.Rating >= 1 && r.Rating <= 5)
                                .AverageAsync(r => (decimal?)r.Rating, stoppingToken);

                            rating = averageRating.HasValue ? (int)Math.Round(averageRating.Value) : DefaultRating;
                        }

                        await packageTripRepository.GetTableNoTracking()
                            .Where(pt => pt.Id == packageTrip.Id)
                            .ExecuteUpdateAsync(pt => pt.SetProperty(x => x.Rate, rating), stoppingToken);

                        _logger.LogInformation("Updated Rating for PackageTrip Id: {Id} to {Rating}", packageTrip.Id, rating);
                    }
                }

                _logger.LogInformation("Completed PromotionAndRatingUpdateService at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PromotionAndRatingUpdateService");
            }

            // الانتظار لمدة 24 ساعة قبل التحقق مرة أخرى
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}