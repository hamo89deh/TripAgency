using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.NewFolder1;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TripDate.Commands;
using TripAgency.Service.Feature.TripDate.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class PackageTripDateService : ReadAndAddService<PackageTripDate, GetPackageTripDateByIdDto, GetPackageTripDatesDto, AddPackageTripDateDto>, IPackageTripDateService
    {
        private IPackageTripDateRepositoryAsync _packageTripDateRepository { get; }
        private IPackageTripRepositoryAsync _packageTripRepositoryAsync { get; }
        private IBookingTripRepositoryAsync _bookingTripRepositoryAsync { get; }
        public ICurrentUserService _currentUserService { get; }
        public IPaymentDiscrepancyReportRepositoryAsync _paymentReportRepositoryAsync { get; }
        public IPaymentRepositoryAsync _paymentRepositoryAsync { get; }
        public IRefundRepositoryAsync _refundRepositoryAsync { get; }
        private INotificationService _notificationServiceAsync { get; }
        private IMapper _mapper { get; }

        public PackageTripDateService(IPackageTripDateRepositoryAsync tripDateRepository,
                              IMapper mapper,
                              IPackageTripRepositoryAsync packageTripRepository,
                              INotificationService notificationServiceAsync,
                              IBookingTripRepositoryAsync bookingTripRepository,
                              ICurrentUserService currentUserService,
                              IPaymentDiscrepancyReportRepositoryAsync paymentDiscrepancyReportRepositoryAsync,
                              IPaymentRepositoryAsync paymentRepositoryAsync,
                              IRefundRepositoryAsync refundRepositoryAsync
                              ) : base(tripDateRepository, mapper)
        {
            _packageTripDateRepository = tripDateRepository;
            _mapper = mapper;
            _packageTripRepositoryAsync = packageTripRepository;
            _notificationServiceAsync = notificationServiceAsync;
            _bookingTripRepositoryAsync = bookingTripRepository;
            _currentUserService = currentUserService;
            _paymentReportRepositoryAsync = paymentDiscrepancyReportRepositoryAsync;
            _paymentRepositoryAsync = paymentRepositoryAsync;
            _refundRepositoryAsync = refundRepositoryAsync;
        }
        public override async Task<Result<GetPackageTripDateByIdDto>> CreateAsync(AddPackageTripDateDto AddDto)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                          .Where(p => p.Id == AddDto.PackageTripId)
                                                          .Include(p => p.PackageTripDestinations)
                                                          .ThenInclude(pd => pd.PackageTripDestinationActivities)
                                                          .FirstOrDefaultAsync();
            if (packageTrip is null)
            {
                return Result<GetPackageTripDateByIdDto>.NotFound($"Not Found PackageTrip With Id : {AddDto.PackageTripId}");
            }
            // التحقق من المدة
            var tripDuration = (AddDto.EndPackageTripDate - AddDto.StartPackageTripDate).Days;
            var expectedDuration = packageTrip.Duration;
            if (Math.Abs(tripDuration - expectedDuration) > 1)
            {
                //_logger.LogWarning("Duration validation failed for PackageTripId: {PackageTripId}. Expected: {ExpectedDuration} days, Got: {TripDuration} days",
                  //  AddDto.PackageTripId, expectedDuration, tripDuration);
                return Result<GetPackageTripDateByIdDto>.BadRequest(
                    $"The duration between StartPackageTripDate and EndPackageTripDate ({tripDuration} days) must be within ±1 day of the PackageTrip duration ({expectedDuration} days) for PackageTripId: {AddDto.PackageTripId}.");
            }

            if (!packageTrip.PackageTripDestinations.Any())
            {
                return Result<GetPackageTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Destinations to this package");
            }

            if (!packageTrip.PackageTripDestinations.Select(pd => pd.PackageTripDestinationActivities).Any())
            {
                return Result<GetPackageTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Activities to destinations[ {string.Join(',', packageTrip.PackageTripDestinations.Select(d => d.DestinationId))} ]");
            }

            var NotFoundPackageTripDestinationActivities = packageTrip.PackageTripDestinations
                                                            .Where(d => d.PackageTripDestinationActivities.Count() == 0);

            if (NotFoundPackageTripDestinationActivities.Any())
            {
                return Result<GetPackageTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Activities to destinations[ {string.Join(',', NotFoundPackageTripDestinationActivities.Select(d => d.DestinationId))} ]");

            }

            var packageTripDate = new PackageTripDate()
            {
                AvailableSeats = packageTrip.MaxCapacity,
                StartBookingDate = AddDto.StartBookingDate,
                EndBookingDate = AddDto.EndBookingDate,
                CreateDate = DateTime.UtcNow,
                IsAvailable = true,
                StartPackageTripDate = AddDto.StartPackageTripDate,
                EndPackageTripDate = AddDto.EndPackageTripDate,
                PackageTripId = packageTrip.Id,
                Status = PackageTripDateStatus.Draft
            };

            await _packageTripDateRepository.AddAsync(packageTripDate);

            var returnResult = _mapper.Map<GetPackageTripDateByIdDto>(packageTripDate);

            return Result<GetPackageTripDateByIdDto>.Success(returnResult);
        }

        public async Task<Result> UpdateStatusTripDate(UpdatePackageTripDateDto updateTripDateDto)
        {
            var packageTripDate = await _packageTripDateRepository.GetTableNoTracking()
                                                         .Where(d => d.Id == updateTripDateDto.Id)
                                                         .Include(tpd => tpd.PackageTrip)
                                                         .ThenInclude(p => p.PackageTripDestinations)
                                                         .ThenInclude(p => p.PackageTripDestinationActivities)
                                                         .FirstOrDefaultAsync();
            if (packageTripDate is null)
                return Result.NotFound($"Not Found Trip Date with Id : {updateTripDateDto.Id}");

            if (packageTripDate.PackageTrip.PackageTripDestinations.Count() == 0)
            {
                return Result.BadRequest($"Cann't Update Status Package Trip Date Before Add Destinations And Activities ");
            }

            if (!packageTripDate.PackageTrip.PackageTripDestinations.All(ptd => ptd.PackageTripDestinationActivities.Any()))
            {
                return Result.BadRequest($"Cann't Update Status Package Trip Before Add Activities For Destinations : {string.Join(',', packageTripDate.PackageTrip.PackageTripDestinations.Where(ptd => ptd.PackageTripDestinationActivities.Count() == 0).Select(d => d.DestinationId))}");
            }

            if (!CanChangeStatus(packageTripDate.Status, Global.ConvertenUpdatePackageTripDataStatusDtoToPackageTripDataStatusDto(updateTripDateDto.Status)))
            {
                return Result.BadRequest($"Cannot change status from {packageTripDate.Status} to {updateTripDateDto.Status}.");
            }
            if (updateTripDateDto.Status == enUpdatePackageTripDataStatusDto.Published)
                return await PublishPackageTripDateAsync(packageTripDate.Id);

            if (updateTripDateDto.Status == enUpdatePackageTripDataStatusDto.BookingClosed)
                return await BookingClosePacakgeTripDateAsync(packageTripDate.Id);

            if (updateTripDateDto.Status == enUpdatePackageTripDataStatusDto.Cancelled)
                return await CancelPacakgeTripDateAsync(packageTripDate.Id);

            return Result.BadRequest("Failed");

        }
        public bool CanChangeStatus(PackageTripDateStatus currentStatus, PackageTripDateStatus newStatus)
        {
            switch (currentStatus)
            {
                case PackageTripDateStatus.Draft:
                    return newStatus == PackageTripDateStatus.Published ||
                           newStatus == PackageTripDateStatus.BookingClosed ||
                           newStatus == PackageTripDateStatus.Cancelled;

                case PackageTripDateStatus.Published:
                    return newStatus == PackageTripDateStatus.BookingClosed ||
                           newStatus == PackageTripDateStatus.Cancelled ||
                           newStatus == PackageTripDateStatus.Full;

                case PackageTripDateStatus.BookingClosed:
                    return newStatus == PackageTripDateStatus.Published ||
                           newStatus == PackageTripDateStatus.Ongoing ||
                           newStatus == PackageTripDateStatus.Cancelled;

                case PackageTripDateStatus.Full:
                    return newStatus == PackageTripDateStatus.BookingClosed || newStatus == PackageTripDateStatus.Cancelled;

                case PackageTripDateStatus.Ongoing:
                    return newStatus == PackageTripDateStatus.Completed;

                default:
                    return false; // الحالات الأخرى لا يمكن تغييرها
            }
        }
        private async Task<Result> BookingClosePacakgeTripDateAsync(int packageTripDateId)
        {

            var packageTripDate = await _packageTripDateRepository.GetTableNoTracking()
                                                    .FirstOrDefaultAsync(td => td.Id == packageTripDateId);

            if (packageTripDate == null)
            {
                return Result.NotFound($"Not Found PackageTripDate With Id : {packageTripDateId}.");
            }
            packageTripDate.Status = PackageTripDateStatus.BookingClosed;
            await _packageTripDateRepository.UpdateAsync(packageTripDate);
            return Result.Success(" BookingClose the packageTrip Success");

        }
        private async Task<Result> CancelPacakgeTripDateAsync(int packageTripDateId, string? cancellationReason="")
        {
            using var transaction = await _packageTripRepositoryAsync.BeginTransactionAsync();

            // _logger.LogInformation("CancelTripDate: بدء إلغاء تاريخ الرحلة {TripDateId} بواسطة المسؤول {AdminUserId}.", tripDateId, adminUserId);

            try
            {
                var packageTripDate = await _packageTripDateRepository.GetTableNoTracking()
                                                        .Where(td => td.Id == packageTripDateId)
                                                        .Include(td => td.BookingTrips)
                                                        .ThenInclude(td => td.Payment)
                                                        .FirstOrDefaultAsync();

                if (packageTripDate == null)
                {
                    await transaction.RollbackAsync();
                    return Result.NotFound($"Not Found PackageTripDate With Id : {packageTripDateId}.");
                }

                if (packageTripDate.Status == PackageTripDateStatus.Cancelled ||
                    packageTripDate.Status == PackageTripDateStatus.Completed ||
                    packageTripDate.Status == PackageTripDateStatus.Ongoing)
                {
                    //_logger.LogWarning("CancelTripDate: تاريخ الرحلة {TripDateId} هو بالفعل بحالة {Status}.", tripDateId, tripDate.Status);
                    await transaction.CommitAsync(); // لا حاجة للتراجع، لا تغييرات فعلية
                    return Result.BadRequest($"The PackageTripDate With Id : {packageTripDate.Id} is in '{packageTripDate.Status.ToString()} Condition' that connot Be Canceled");
                }

                packageTripDate.Status = PackageTripDateStatus.Cancelled;
                packageTripDate.IsAvailable = false;
                await _packageTripDateRepository.UpdateAsync(packageTripDate);

                var admin = await _currentUserService.GetUserAsync();

                var affectedBookings = packageTripDate.BookingTrips.ToList(); // نسخ القائمة لتجنب التعديل أثناء المرور

                foreach (var booking in affectedBookings)
                {
                    if (booking.BookingStatus == BookingStatus.Pending)
                    {
                        booking.BookingStatus = BookingStatus.Cancelled;
                        await _bookingTripRepositoryAsync.UpdateAsync(booking);

                        if (string.IsNullOrEmpty(booking.Payment.TransactionId))
                        {

                            // 3.3. إرسال إشعار للعميل المتأثر
                            await _notificationServiceAsync.CreateInAppNotificationAsync(
                                booking.UserId,
                                "Cancelling Booking " + booking.Id,
                                $"Cancelled Booking {booking.Id} Because the admin Cancelling The PackageTrip Date: {booking.PackageTripDate.StartPackageTripDate}",
                                "TripCancelled",
                                booking.Id.ToString());
                        }
                        else
                        {
                            var report = new PaymentDiscrepancyReport
                            {
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                PaymentId = booking.Payment.Id,
                                ReportDate = DateTime.Now,
                                ReportedPaymentDateTime = DateTime.Now,
                                Status = PaymentDiscrepancyStatusEnum.PendingReview,
                                CustomerNotes = $"Package Tirp With Id : {packageTripDateId} Cancel By Admin",
                                ReportedPaidAmount = booking.ActualPrice,
                                ReportedTransactionRef = booking.Payment.TransactionId,
                                UserId = admin.Id,
                            };
                            await _paymentReportRepositoryAsync.AddAsync(report);

                            await _notificationServiceAsync.CreateInAppNotificationAsync(
                               booking.UserId,
                               "Cancelling Booking " + booking.Id,
                               $"Cancelled Booking {booking.Id} Because the admin Cancelling The PackageTrip Date: {booking.PackageTripDate.StartPackageTripDate} The reported Payment is in Case of Review",
                               "TripCancelled",
                               booking.Id.ToString());
                        }

                        //_logger.LogInformation("CancelTripDate: تم إلغاء الحجز {BookingId} لأنه مرتبط بـ TripDate الملغاة.", booking.Id);

                    }
                    else if (booking.BookingStatus == BookingStatus.Completed)
                    {
                        booking.BookingStatus = BookingStatus.Cancelled;
                        await _bookingTripRepositoryAsync.UpdateAsync(booking);
                        
                        var refund = new Refund
                        {
                            PaymentId = booking.Payment.Id,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            TransactionReference = booking.Payment.TransactionId,
                            Status = RefundStatus.Pending,
                            Amount = booking.Payment.Amount,
                        };
                        await _refundRepositoryAsync.AddAsync(refund);

                    }
                    var affectPaymnet = booking.Payment;
                    affectPaymnet.PaymentStatus = PaymentStatus.Cancelled;
                    await _paymentRepositoryAsync.UpdateAsync(affectPaymnet);

                }

                await transaction.CommitAsync();
                //_logger.LogInformation("CancelTripDate: تم إلغاء تاريخ الرحلة {TripDateId} بنجاح. عدد الحجوزات المتأثرة: {Count}.", tripDateId, affectedBookings.Count);
                return Result.Success($"Cancelling The PackageTrip Date: {packageTripDate.StartPackageTripDate} .");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                //  _logger.LogError(ex, "CancelTripDate: خطأ غير متوقع أثناء إلغاء تاريخ الرحلة {TripDateId}.", tripDateId);
                throw ex;
            }
        }
        private async Task<Result> PublishPackageTripDateAsync(int packageTripDateId)
        {
            using var transaction = await _packageTripRepositoryAsync.BeginTransactionAsync();

            // _logger.LogInformation("CancelTripDate: بدء إلغاء تاريخ الرحلة {TripDateId} بواسطة المسؤول {AdminUserId}.", tripDateId, adminUserId);

            try
            {
                var packageTripDate = await _packageTripDateRepository.GetTableNoTracking()
                                                        .FirstOrDefaultAsync(td => td.Id == packageTripDateId);


                if (packageTripDate == null)
                {
                    await transaction.RollbackAsync();
                    return Result.NotFound($"Not Found PackageTripDate With Id : {packageTripDateId}.");
                }
                if (packageTripDate.Status == PackageTripDateStatus.Draft)
                {
                    packageTripDate.Status = PackageTripDateStatus.Published;
                    await _packageTripDateRepository.UpdateAsync(packageTripDate);
                }
                if (packageTripDate.Status == PackageTripDateStatus.BookingClosed)
                {
                    if (packageTripDate.EndBookingDate <= DateTime.Now)
                    {
                        return Result.BadRequest($"Cannot RePublished the PackageTrip With id: {packageTripDateId} after booking period has ended");
                    }  
                    if (packageTripDate.AvailableSeats == 0)
                    {
                        return Result.BadRequest($"Cannot RePublished the PackageTrip With id: {packageTripDateId} Because there are no seats available");

                    }
                    packageTripDate.Status = PackageTripDateStatus.Published;
                    await _packageTripDateRepository.UpdateAsync(packageTripDate);
                }
                await transaction.CommitAsync();
                return Result.Success("Publish the packageTrip Success");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                //  _logger.LogError(ex, "CancelTripDate: خطأ غير متوقع أثناء إلغاء تاريخ الرحلة {TripDateId}.", tripDateId);
                return Result.Failure("Internal Error", failureType: ResultFailureType.InternalError);
            }
        }
        public async Task<Result<IEnumerable<GetPackageTripDateByIdDto>>> GetDateForPackageTrip(int packageTripId, PackageTripDateStatus? status)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                                .FirstOrDefaultAsync(x=> x.Id == packageTripId);
           if (packageTrip is null)
                return Result<IEnumerable<GetPackageTripDateByIdDto>>.NotFound($"Not Found PackageTrip with id {packageTripId}");
            var datePackageTripDate = _packageTripDateRepository.GetTableNoTracking()
                                                                 .Where(p => p.PackageTripId == packageTripId);
            if (status is not null)
                datePackageTripDate = datePackageTripDate.Where(d => d.Status == status);
            var result = await datePackageTripDate.Select(x => new GetPackageTripDateByIdDto
            {
                Status = x.Status,
                AvailableSeats = x.AvailableSeats,
                EndBookingDate = x.EndBookingDate,
                EndTripDate = x.EndBookingDate,
                Id = x.Id,
                PackageTripId = packageTripId,
                StartBookingDate = x.StartBookingDate,
                StartTripDate = x.StartBookingDate
            }).ToListAsync();
            if (!result.Any())
                return Result<IEnumerable<GetPackageTripDateByIdDto>>.NotFound($"Not Found Any date {status} for PackageTrip with id {packageTripId}");
            return Result<IEnumerable<GetPackageTripDateByIdDto>>.Success(result);


        }
    }
}

