using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Infrastructure.Abstracts;
using Microsoft.Extensions.Logging;

public class PackageTripDateStatusUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PackageTripDateStatusUpdateService> _logger;

    public PackageTripDateStatusUpdateService(IServiceProvider serviceProvider, ILogger<PackageTripDateStatusUpdateService> logger)
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
            _logger.LogInformation("Waiting until {NextRun} UTC to run PackageTripDateStatusUpdateService again", DateTime.UtcNow.Add(delay));
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task ExecuteTask(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting PackageTripDateStatusUpdateService at {Time} UTC", DateTime.UtcNow);
            using (var scope = _serviceProvider.CreateScope())
            {
                var packageTripDateRepository = scope.ServiceProvider.GetRequiredService<IPackageTripDateRepositoryAsync>();

                var packageTripDates = await packageTripDateRepository.GetTableNoTracking()
                    .Where(ptd => ptd.Status == PackageTripDateStatus.Published ||
                                  ptd.Status == PackageTripDateStatus.BookingClosed ||
                                  ptd.Status == PackageTripDateStatus.Ongoing ||
                                  ptd.Status == PackageTripDateStatus.Full)
                    .ToListAsync(stoppingToken);

                foreach (var packageTripDate in packageTripDates)
                {
                    if (packageTripDate.Status == PackageTripDateStatus.Cancelled)
                    {
                        _logger.LogInformation("Skipping Cancelled PackageTripDate Id: {Id}", packageTripDate.Id);
                        continue;
                    }

                    var today = DateTime.UtcNow.Date;

                    if (today >= packageTripDate.EndBookingDate.Date &&
                        (packageTripDate.Status == PackageTripDateStatus.Published ||
                         packageTripDate.Status == PackageTripDateStatus.Full))
                    {
                        packageTripDate.Status = PackageTripDateStatus.BookingClosed;
                        await packageTripDateRepository.UpdateAsync(packageTripDate);
                        _logger.LogInformation("Updated PackageTripDate Id: {Id} to BookingClosed", packageTripDate.Id);
                    }
                    else if (today == packageTripDate.StartPackageTripDate.Date &&
                             (packageTripDate.Status == PackageTripDateStatus.BookingClosed ||
                              packageTripDate.Status == PackageTripDateStatus.Published ||
                              packageTripDate.Status == PackageTripDateStatus.Full))
                    {
                        packageTripDate.Status = PackageTripDateStatus.Ongoing;
                        await packageTripDateRepository.UpdateAsync(packageTripDate);
                        _logger.LogInformation("Updated PackageTripDate Id: {Id} to Ongoing", packageTripDate.Id);
                    }
                    else if (today == packageTripDate.EndPackageTripDate.Date &&
                             (packageTripDate.Status == PackageTripDateStatus.Ongoing ||
                              packageTripDate.Status == PackageTripDateStatus.BookingClosed ||
                              packageTripDate.Status == PackageTripDateStatus.Published ||
                              packageTripDate.Status == PackageTripDateStatus.Full))
                    {
                        packageTripDate.Status = PackageTripDateStatus.Completed;
                        await packageTripDateRepository.UpdateAsync(packageTripDate);
                        _logger.LogInformation("Updated PackageTripDate Id: {Id} to Completed", packageTripDate.Id);
                    }
                }
            }

            _logger.LogInformation("Completed PackageTripDateStatusUpdateService at {Time} UTC", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PackageTripDateStatusUpdateService");
        }
    }

    private static TimeSpan CalculateDelayToNextMidnight()
    {
        var now = DateTime.UtcNow;
        var nextMidnight = now.Date.AddDays(1);
        return nextMidnight - now;
    }
}