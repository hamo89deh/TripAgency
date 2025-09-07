using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TripAgency.Infrastructure.Abstracts;
using Microsoft.Extensions.Logging;
public class OfferUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OfferUpdateService> _logger;

    public OfferUpdateService(IServiceProvider serviceProvider, ILogger<OfferUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // تنفيذ فوري عند بدء الخدمة
            await ExecuteTask(stoppingToken);

            // حساب التأخير حتى منتصف الليل القادم
            var delay = CalculateDelayToNextMidnight();
            _logger.LogInformation("Waiting until {NextRun} UTC to run OfferUpdateService again", DateTime.UtcNow.Add(delay));
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task ExecuteTask(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting OfferUpdateService at {Time} UTC", DateTime.UtcNow);

            using (var scope = _serviceProvider.CreateScope())
            {
                var packageTripOffersRepository = scope.ServiceProvider.GetRequiredService<IPackageTripOffersRepositoryAsync>();

                // تعطيل العروض المنتهية
                var updatedCount = await packageTripOffersRepository.GetTableNoTracking()
                    .Include(x => x.Offer)
                    .Where(x => x.IsApply && x.Offer.IsActive && x.Offer.EndDate < DateOnly.FromDateTime(DateTime.UtcNow))
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsApply, false), stoppingToken);

                _logger.LogInformation("Disabled {Count} expired offers.", updatedCount);

                // ضمان وجود عرض واحد نشط لكل PackageTrip
                var packageTripsWithMultipleOffers = await packageTripOffersRepository.GetTableNoTracking()
                    .Include(x => x.Offer)
                    .Where(x => x.IsApply && x.Offer.IsActive && x.Offer.EndDate >= DateOnly.FromDateTime(DateTime.UtcNow) && x.Offer.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow))
                    .GroupBy(x => x.PackageTripId)
                    .Where(g => g.Count() > 1)
                    .Select(g => new { PackageTripId = g.Key, Offers = g.OrderByDescending(p => p.Offer.DiscountPercentage).ThenByDescending(p => p.Offer.StartDate).ToList() })
                    .ToListAsync(stoppingToken);
               

                _logger.LogInformation("Disabled {Count} expired offers.", updatedCount);

               
                foreach (var group in packageTripsWithMultipleOffers)
                {
                    var bestOffer = group.Offers.First();
                    var offersToDisable = group.Offers.Skip(1);

                    foreach (var offer in offersToDisable)
                    {
                        await packageTripOffersRepository.GetTableNoTracking()
                            .Where(x => x.PackageTripId == group.PackageTripId && x.OfferId == offer.OfferId)
                            .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsApply, false), stoppingToken);
                        _logger.LogInformation("Disabled lower-priority offer {OfferId} for PackageTrip {PackageTripId}.", offer.OfferId, group.PackageTripId);
                    }
                }
            }

            _logger.LogInformation("Completed OfferUpdateService at {Time} UTC", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OfferUpdateService");
        }
    }

    private static TimeSpan CalculateDelayToNextMidnight()
    {
        var now = DateTime.UtcNow;
        var nextMidnight = now.Date.AddDays(1);
        return nextMidnight - now;
    }
}

public class OfferAndRatingUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OfferAndRatingUpdateService> _logger;
    private readonly int DefaultRating = 5;
    private readonly int MinimumReviews = 5;

    public OfferAndRatingUpdateService(IServiceProvider serviceProvider, ILogger<OfferAndRatingUpdateService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // تنفيذ فوري عند بدء الخدمة
            await ExecuteTask(stoppingToken);

            // حساب التأخير حتى منتصف الليل القادم
            var delay = CalculateDelayToNextMidnight();
            _logger.LogInformation("Waiting until {NextRun} UTC to run OfferAndRatingUpdateService again", DateTime.UtcNow.Add(delay));
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task ExecuteTask(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting OfferAndRatingUpdateService at {Time} UTC", DateTime.UtcNow);

            using (var scope = _serviceProvider.CreateScope())
            {
                var offerRepository = scope.ServiceProvider.GetRequiredService<IOffersRepositoryAsync>();
                var tripReviewRepository = scope.ServiceProvider.GetRequiredService<ITripReviewRepositoryAsync>();
                var packageTripRepository = scope.ServiceProvider.GetRequiredService<IPackageTripRepositoryAsync>();

                // تحديث العروض الترويجية
                var updatedOffersCount = await offerRepository.GetTableNoTracking()
                    .Where(p => p.IsActive && p.EndDate < DateOnly.FromDateTime(DateTime.Now))
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsActive, false), stoppingToken);

                _logger.LogInformation("Updated {Count} expired promotions.", updatedOffersCount);

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

                    int rating = DefaultRating;
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

            _logger.LogInformation("Completed OfferAndRatingUpdateService at {Time} UTC", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OfferAndRatingUpdateService");
        }
    }

    private static TimeSpan CalculateDelayToNextMidnight()
    {
        var now = DateTime.Now;
        var nextMidnight = now.Date.AddDays(1); // منتصف الليل القادم بتوقيت UTC
        return nextMidnight - now;
    }
}