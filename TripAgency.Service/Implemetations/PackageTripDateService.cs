using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.NetworkInformation;
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
        private IPackageTripDateRepositoryAsync _tripDateRepositoryAsync { get; }
        private IPackageTripRepositoryAsync _packageTripRepositoryAsync { get; }  
        private IBookingTripRepositoryAsync _bookingTripRepositoryAsync { get; }
        private INotificationService _notificationServiceAsync { get; }
        private IMapper _mapper { get; }

        public PackageTripDateService(IPackageTripDateRepositoryAsync tripDateRepository,
                              IMapper mapper,
                              IPackageTripRepositoryAsync packageTripRepository,
                              INotificationService notificationServiceAsync,
                              IBookingTripRepositoryAsync bookingTripRepository
                              ) : base(tripDateRepository, mapper)
        {
            _tripDateRepositoryAsync = tripDateRepository;
            _mapper = mapper;
            _packageTripRepositoryAsync = packageTripRepository;
            _notificationServiceAsync = notificationServiceAsync;
            _bookingTripRepositoryAsync = bookingTripRepository;
        }
        public override async Task<Result<GetPackageTripDateByIdDto>> CreateAsync(AddPackageTripDateDto AddDto)
        {
            var packageTrip = await _packageTripRepositoryAsync.GetTableNoTracking()
                                                          .Where(p=>p.Id == AddDto.PackageTripId)
                                                          .Include(p=>p.PackageTripDestinations)
                                                          .ThenInclude(pd=>pd.PackageTripDestinationActivities)
                                                          .FirstOrDefaultAsync();
            if(packageTrip is null)
            {
                return Result<GetPackageTripDateByIdDto>.NotFound($"Not Found PackageTrip With Id : {AddDto.PackageTripId}");
            }
           
            if(!packageTrip.PackageTripDestinations.Any())
            {
                return Result<GetPackageTripDateByIdDto>.BadRequest($"Cannot Add Date to PackageTrip with id : {packageTrip.Id} Before Add Destinations to this package");
            }

            if(!packageTrip.PackageTripDestinations.Select(pd => pd.PackageTripDestinationActivities).Any())
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
                StartBookingDate =  AddDto.StartBookingDate,
                EndBookingDate = AddDto.EndBookingDate,
                CreateDate = DateTime.UtcNow,
                IsAvailable = true,
                StartPackageTripDate = AddDto.StartPackageTripDate,
                EndPackageTripDate = AddDto.EndPackageTripDate, 
                PackageTripId = packageTrip.Id,
                Status = PackageTripDateStatus.Draft           
            };

            await _tripDateRepositoryAsync.AddAsync(packageTripDate);

            var returnResult = _mapper.Map<GetPackageTripDateByIdDto>(packageTripDate);

            return Result<GetPackageTripDateByIdDto>.Success(returnResult);
        }

        public async Task<Result> UpdateStatusTripDate(UpdatePackageTripDateDto updateTripDateDto)
        {
            var packageTripDate = await _tripDateRepositoryAsync.GetTableAsTracking()
                                                         .Where(d=>d.Id == updateTripDateDto.Id)
                                                         .Include(tpd=>tpd.PackageTrip)
                                                         .ThenInclude(p=>p.PackageTripDestinations)
                                                         .ThenInclude(p=>p.PackageTripDestinationActivities)
                                                         .FirstOrDefaultAsync();
            if( packageTripDate is null)
                return Result.NotFound($"Not Found Trip Date with Id : {updateTripDateDto.Id}");

           
            if (!CanChangeStatus(packageTripDate.Status, Global.ConvertenUpdatePackageTripDataStatusDtoToPackageTripDataStatusDto(updateTripDateDto.Status)))
            {
                return Result.BadRequest($"Cannot change status from {packageTripDate.Status} to {updateTripDateDto.Status}.");
            }

            if (packageTripDate.Status == PackageTripDateStatus.Draft &&
               updateTripDateDto.Status == enUpdatePackageTripDataStatusDto.Published)
            {
                
                if (packageTripDate.PackageTrip.PackageTripDestinations.Count() == 0)
                {
                    return Result.BadRequest($"Cann't Update Status Package Trip Before Add Destinations And Activities ");
                }

                if (!packageTripDate.PackageTrip.PackageTripDestinations.All(ptd => ptd.PackageTripDestinationActivities.Any()))
                {
                    return Result.BadRequest($"Cann't Update Status Package Trip Before Add Activities For Destinations : {string.Join(',', packageTripDate.PackageTrip.PackageTripDestinations.Where(ptd => ptd.PackageTripDestinationActivities.Count() == 0).Select(d => d.DestinationId))}");
                }
            }

            if (packageTripDate.Status == PackageTripDateStatus.BookingClosed &&
                updateTripDateDto.Status == enUpdatePackageTripDataStatusDto.Published)
            {

                if (packageTripDate.AvailableSeats <= 0)
                {
                    return Result.BadRequest("Cann't republished Because the Package of the Trip is full");
                }
            
                if (packageTripDate.EndBookingDate <= DateTime.UtcNow)
                {
                    return Result.BadRequest("Cann't republished Because the Booking expiring");
                }
            }

            packageTripDate.Status = Global.ConvertenUpdatePackageTripDataStatusDtoToPackageTripDataStatusDto(updateTripDateDto.Status);
            await _tripDateRepositoryAsync.SaveChangesAsync();

            await ExecuteStatusSpecificActions(packageTripDate.Id, packageTripDate.Status);


            return Result.Success();

        }
        public bool CanChangeStatus(PackageTripDateStatus currentStatus, PackageTripDateStatus newStatus)
        {
            switch (currentStatus)
            {
                case PackageTripDateStatus.Draft:
                    return newStatus == PackageTripDateStatus.Published || newStatus == PackageTripDateStatus.Cancelled;
                case PackageTripDateStatus.Published:
                    return newStatus == PackageTripDateStatus.BookingClosed ||
                           newStatus == PackageTripDateStatus.Cancelled ||
                           newStatus == PackageTripDateStatus.Full;
                case PackageTripDateStatus.BookingClosed:
                    return newStatus == PackageTripDateStatus.Ongoing ||
                           newStatus == PackageTripDateStatus.Cancelled;
                case PackageTripDateStatus.Full:
                    return newStatus == PackageTripDateStatus.BookingClosed || newStatus == PackageTripDateStatus.Cancelled;
                case PackageTripDateStatus.Ongoing:
                    return newStatus == PackageTripDateStatus.Completed;
                default:
                    return false; // الحالات الأخرى لا يمكن تغييرها
            }
        }
        public async Task ExecuteStatusSpecificActions(int packageTripDateId, PackageTripDateStatus newStatus)
        {
            switch (newStatus)
            {
                //case PackageTripDateStatus.Cancelled:
                //    await _notificationServiceAsync.NotifyTripCancellation(packageTripDateId);

                //    await ProcessRefundsForCancelledTrip(packageTripDateId);
                //    break;

                //case PackageTripDateStatus.Completed:
                //    await _notificationServiceAsync.NotifyTripCompletion(packageTripDateId);
                //    break;
            }
        }
        //TODO
        public async Task ProcessRefundsForCancelledTrip(int packageTripDateId)
        {
            // كود استرداد المبالغ (مثال باستخدام Stripe)
            // var bookings = _bookingTripRepositoryAsync.GetBookingTrips(packageTripDateId, PaymentStatus.Paid).ToList();

            //foreach (var booking in bookings)
            //{
            //    //var refundResult = await _paymentGateway.ProcessRefundAsync(
            //    //    booking.PaymentTransactionId,
            //    //    booking.AmountPaid);

            //    //booking.PaymentStatus = refundResult.Success ?
            //    //    PaymentStatus.Refunded :
            //    //    PaymentStatus.RefundFailed;
            //}
        }
        //TODO
        public async Task<Result> CancelPacakgeTripDateAsync(int packageTripDateId, int adminUserId, string cancellationReason)
        {
             using var transaction = await _packageTripRepositoryAsync.BeginTransactionAsync();
           
            // _logger.LogInformation("CancelTripDate: بدء إلغاء تاريخ الرحلة {TripDateId} بواسطة المسؤول {AdminUserId}.", tripDateId, adminUserId);

            try
            {
                var packageTripDate = await _tripDateRepositoryAsync.GetTableNoTracking() // GetTable للحصول على كيان متتبع
                                                        .Where(td => td.Id == packageTripDateId)
                                                        .Include(td => td.BookingTrips) // **** تضمين الحجوزات المرتبطة ****
                                                        .FirstOrDefaultAsync();

                if (packageTripDate == null)
                {
                    await transaction.RollbackAsync();
                    return Result.NotFound($"تاريخ الرحلة بالرقم التعريفي {packageTripDateId} غير موجود.");
                }

                // 1. التحقق من أن تاريخ الرحلة ليس ملغياً بالفعل أو مكتمل
                if (packageTripDate.Status == PackageTripDateStatus.Cancelled ||
                    packageTripDate.Status == PackageTripDateStatus.Completed || 
                    packageTripDate.Status == PackageTripDateStatus.Ongoing  )
                {
                    //_logger.LogWarning("CancelTripDate: تاريخ الرحلة {TripDateId} هو بالفعل بحالة {Status}.", tripDateId, tripDate.Status);
                    await transaction.CommitAsync(); // لا حاجة للتراجع، لا تغييرات فعلية
                    return Result.BadRequest($"تاريخ الرحلة {packageTripDate.Id} هو بالفعل بحالة '{packageTripDate.Status.ToString()}' ولا يمكن إلغاؤه.");
                }

                // 2. تحديث حالة TripDate إلى 'Cancelled'
                packageTripDate.Status = PackageTripDateStatus.Cancelled;
                packageTripDate.IsAvailable = false;
                await _tripDateRepositoryAsync.UpdateAsync(packageTripDate);

                // 3. معالجة الحجوزات المرتبطة بهذا TripDate
                // جلب المسؤول الذي قام بالإلغاء (للتدقيق في سجلات Refunds)
                var adminUser = 2; //Todo
                var adminUserName = "AdminSystem";

                var affectedBookings = packageTripDate.BookingTrips.ToList(); // نسخ القائمة لتجنب التعديل أثناء المرور

                foreach (var booking in affectedBookings)
                {
                    // 3.1. إلغاء الحجز (إذا لم يكن ملغى أو مكتمل)
                    if (booking.BookingStatus == BookingStatus.Pending)
                    {
                        booking.BookingStatus = BookingStatus.Cancelled;
                        await _bookingTripRepositoryAsync.UpdateAsync(booking);
                        //_logger.LogInformation("CancelTripDate: تم إلغاء الحجز {BookingId} لأنه مرتبط بـ TripDate الملغاة.", booking.Id);
                        
                    }
                    else if (booking.BookingStatus ==BookingStatus.Completed)
                    {
                        // 3.2. إذا كان الحجز مؤكداً، يجب إبلاغ العميل بضرورة الاسترداد (أو استرداد تلقائي)
                        //_logger.LogWarning("CancelTripDate: الحجز {BookingId} كان مؤكداً وسيتم إلغاؤه. يحتاج استرداد مبلغ.", booking.Id);

                        // هنا يتم تسجيل عملية الاسترداد لهذا الحجز المؤكد
                        // ممكن استدعاء دالة RefundBookingAsync بشكل منفصل (بتحتاج تعديل بسيط في التوقيع لتسمح بـ adminUser)
                        // أو تسجيل طلب استرداد لمراجعة المسؤول

                        // لغرض التوضيح في هذه الدالة، سنقوم بإنشاء بلاغ مباشرة وإشعار
                        //var discrepancyReport = new PaymentDiscrepancyReport
                        //{
                        //    BookingId = booking.Id,
                        //    UserId = booking.UserId,
                        //    ReportedTransactionRef = "Trip Cancellation", // ليس رقم عملية دفع
                        //    ReportedPaymentDateTime = DateTime.UtcNow,
                        //    ReportedPaidAmount = booking.ActualPrice,
                        //    CustomerNotes = $"تم إلغاء الرحلة رقم {tripDateId}. يرجى التواصل لاسترداد المبلغ.",
                        //    ReportDate = DateTime.UtcNow,
                        //    Status = (int)PaymentDiscrepancyStatusEnum.PendingReview,
                        //    AdminNotes = $"إلغاء رحلة {tripDateId} تسبب بإلغاء حجز مؤكد {booking.Id}. يتطلب استرداد مبلغ."
                        //};
                        //await _discrepancyReportRepository.AddAsync(discrepancyReport);
                    }

                    // 3.3. إرسال إشعار للعميل المتأثر
                    await _notificationServiceAsync.CreateInAppNotificationAsync(
                        booking.UserId,
                        "إلغاء رحلتك رقم " + packageTripDateId,
                        $"نعتذر، تم إلغاء رحلتك رقم {packageTripDateId} بتاريخ {packageTripDate.StartPackageTripDate.ToShortDateString()}. سبب الإلغاء: {cancellationReason}. يرجى مراجعة حجوزاتك لمعرفة التفاصيل.",
                        "TripCancelled",
                        booking.Id.ToString()
                    );
                }

                await transaction.CommitAsync();
                //_logger.LogInformation("CancelTripDate: تم إلغاء تاريخ الرحلة {TripDateId} بنجاح. عدد الحجوزات المتأثرة: {Count}.", tripDateId, affectedBookings.Count);
                return Result.Success("تم إلغاء تاريخ الرحلة بنجاح.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
              //  _logger.LogError(ex, "CancelTripDate: خطأ غير متوقع أثناء إلغاء تاريخ الرحلة {TripDateId}.", tripDateId);
                return Result.Failure("حدث خطأ داخلي أثناء إلغاء تاريخ الرحلة.");
            }
            
        }
    }  
}

