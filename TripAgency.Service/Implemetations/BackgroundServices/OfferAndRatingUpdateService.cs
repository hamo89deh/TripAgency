using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TripAgency.Infrastructure.Abstracts;
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
            _logger.LogInformation("Waiting until {NextRun}  to run OfferAndRatingUpdateService again", DateTime.Now.Add(delay));
            await Task.Delay(delay, stoppingToken);
        }
    }
    private async Task ExecuteTask(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("StartingService");

            using var scope = _serviceProvider.CreateScope();
            using var transaction = await scope.ServiceProvider
                .GetRequiredService<IPackageTripOffersRepositoryAsync>()
                .BeginTransactionAsync();

            var offerRepository = scope.ServiceProvider.GetRequiredService<IOffersRepositoryAsync>();
            var packageTripOffersRepository = scope.ServiceProvider.GetRequiredService<IPackageTripOffersRepositoryAsync>();
            var tripReviewRepository = scope.ServiceProvider.GetRequiredService<ITripReviewRepositoryAsync>();
            var packageTripRepository = scope.ServiceProvider.GetRequiredService<IPackageTripRepositoryAsync>();

            // تعطيل العروض المنتهية في Offers
            var updatedOffersCount = await offerRepository.GetTableNoTracking()
                .Where(p => p.IsActive && p.EndDate < DateTime.Now)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsActive, false), stoppingToken);
            _logger.LogInformation("DisabledExpiredOffers");

            // تعطيل العروض المنتهية في PackageTripOffers
            var updatedPackageOffersCount = await packageTripOffersRepository.GetTableNoTracking()
                .Include(x => x.Offer)
                .Where(x => x.IsApply && (x.Offer.EndDate < DateTime.Now || !x.Offer.IsActive))
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsApply, false), stoppingToken);
            _logger.LogInformation("DisabledExpiredPackageTripOffers");

            // ضمان وجود عرض واحد نشط لكل PackageTrip
            var packageTripsWithMultipleOffers = await packageTripOffersRepository.GetTableNoTracking()
                .Include(x => x.Offer)
                .Where(x => x.IsApply && x.Offer.IsActive &&
                           x.Offer.EndDate >= DateTime.Now &&
                           x.Offer.StartDate <= DateTime.Now)
                .GroupBy(x => x.PackageTripId)
                .Where(g => g.Count() > 1)
                .Select(g => new
                {
                    PackageTripId = g.Key,
                    BestOfferId = g.OrderByDescending(p => p.Offer.DiscountPercentage)
                                  .ThenByDescending(p => p.Offer.StartDate)
                                  .Select(p => p.OfferId)
                                  .First()
                })
                .ToListAsync(stoppingToken);

            foreach (var group in packageTripsWithMultipleOffers)
            {
                await packageTripOffersRepository.GetTableNoTracking()
                    .Where(x => x.PackageTripId == group.PackageTripId && x.OfferId != group.BestOfferId && x.IsApply)
                    .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsApply, false), stoppingToken);
                _logger.LogInformation("DisabledLowerPriorityOffers");
            }

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
                _logger.LogInformation("UpdatedRating");
            }

            await transaction.CommitAsync(stoppingToken);
            _logger.LogInformation("CompletedService");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ServiceError");
            throw;
        }
    }

    private static TimeSpan CalculateDelayToNextMidnight()
    {
        var now = DateTime.Now;
        var nextMidnight = now.Date.AddDays(1); // منتصف الليل القادم بتوقيت 
        return nextMidnight - now;
    }
}