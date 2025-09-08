using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Feature.Payment;

namespace TripAgency.Service.Implemetations.Payment
{
    public class PaymentService : IPaymentService
    {
        private IBookingTripRepositoryAsync _bookingTripRepository { get; set; }
        private IPackageTripDateRepositoryAsync _packageTripDateRepository { get; set; }
        public IPaymentMethodRepositoryAsync _paymentMethodRepositoryAsync { get; }
        public IPaymentTimerService _paymentTimerService { get; }
        public INotificationService _notificationService { get; }
        public IPaymentGatewayFactory _paymentGatewayFactory { get; }
        public IPaymentDiscrepancyReportRepositoryAsync _discrepancyReportRepositoryAsync { get; }
        public IPaymentRepositoryAsync _paymentRepositoryAsync { get; }
        public IRefundRepositoryAsync _refundRepositoryAsync { get; }
        public IConfiguration _configuration { get; }
        public ICurrentUserService _currentUserService { get; }
        public IMapper _mapper { get; }
        private string baseUrl;
        public PaymentService(IBookingTripRepositoryAsync bookingTripRepository,
                                  IPackageTripDateRepositoryAsync tripDateRepository,
                                  IPaymentMethodRepositoryAsync paymentMethodRepositoryAsync,
                                  IPaymentTimerService paymentTimerService,
                                  IPaymentGatewayFactory paymentGatewayFactory,
                                  INotificationService notificationService,
                                  IPaymentDiscrepancyReportRepositoryAsync discrepancyReportRepositoryAsync,
                                  IPaymentRepositoryAsync paymentRepositoryAsync,
                                  IRefundRepositoryAsync refundRepositoryAsync,
                                  IMapper mapper,
                                  IConfiguration configuration,
                                  ICurrentUserService currentUserService
                                 )
        {
            _bookingTripRepository = bookingTripRepository;
            _packageTripDateRepository = tripDateRepository;
            _paymentMethodRepositoryAsync = paymentMethodRepositoryAsync;
            _paymentTimerService = paymentTimerService;
            _paymentGatewayFactory = paymentGatewayFactory;
            _discrepancyReportRepositoryAsync = discrepancyReportRepositoryAsync;
            _paymentRepositoryAsync = paymentRepositoryAsync;
            _refundRepositoryAsync = refundRepositoryAsync;
            _mapper = mapper;
            _notificationService = notificationService;
            _configuration = configuration;
            _currentUserService = currentUserService;
            baseUrl = configuration["BaseUrl"] ?? throw new InvalidOperationException("Not Found BaseUrl."); ;
        }
        //  دالة لمعالجة انتهاء مهلة الدفع (تستدعى من خدمة المؤقت)
        public async Task<Result> HandlePaymentTimeoutAsync(int bookingId)
        {
            var bookingTrip = await _bookingTripRepository.GetTableNoTracking()
                                                           .Where(b => b.Id == bookingId)
                                                           .Include(p => p.Payment)
                                                           .FirstOrDefaultAsync();

            if (bookingTrip == null)
            {
                // _logger.LogWarning("Timeout: مهلة دفع انتهت لحجز غير موجود: {BookingId}", bookingId); return Result.NotFound("الحجز غير موجود."); 
                return Result.NotFound($"Not Found Booking With Id : {bookingId}");

            }

            // إذا كان الحجز في حالة Pending (لم يتم الدفع)
            if (bookingTrip.BookingStatus == BookingStatus.Pending)
            {
                // _logger.LogInformation("Timeout: الحجز {BookingId} في حالة Pending. جاري الإلغاء.", bookingId);

                bookingTrip.BookingStatus = BookingStatus.Cancelled;
                await _bookingTripRepository.UpdateAsync(bookingTrip);

                var tripDate = await _packageTripDateRepository.GetByIdAsync(bookingTrip.PackageTripDateId);
                if (tripDate != null)
                {
                    tripDate.AvailableSeats += bookingTrip.PassengerCount;
                    if (tripDate.Status == PackageTripDateStatus.Full)
                        tripDate.Status = PackageTripDateStatus.Published;
                    await _packageTripDateRepository.UpdateAsync(tripDate);
                }


                bookingTrip.Payment.PaymentStatus = PaymentStatus.Cancelled;
                await _paymentRepositoryAsync.UpdateAsync(bookingTrip.Payment);


                // _logger.LogInformation("Timeout: الحجز {BookingId} تم إلغاؤه تلقائياً بسبب انتهاء مهلة الدفع.", bookingId);
                return Result.Success("The Booking Cancelling due to deadline.");
            }

            // _logger.LogInformation("Timeout: مهلة دفع انتهت لحجز {BookingId} ليس في حالة Pending (الحالة الحالية: {Status}). لا حاجة لإلغاء إضافي.", bookingId, bookingTrip.BookingStatus);
            return Result.Success("لا حاجة لعملية إلغاء، الحجز ليس في حالة Pending.");
        }
        // 4. تسجيل إشعار من العميل بالدفع اليدوي
        public async Task<Result> SubmitManualPaymentNotificationAsync(ManualPaymentDetailsDto details)
        {
            var bookingTrip = await _bookingTripRepository.GetTableNoTracking()
                                                          .Where(d => d.Id == details.BookingId)
                                                          .Include(b => b.Payment)
                                                          .ThenInclude(p => p.PaymentMethod)
                                                          .Include(b => b.PackageTripDate)
                                                          .FirstOrDefaultAsync();

            // 1. التحقق من أن الحجز  
            if (bookingTrip == null)
            {
                return Result.NotFound($"Not Found Booking with id : {details.BookingId}");
            }

            // 2. التحقق من أن الحجز يخص المستخدم الحالي 
            var user = await _currentUserService.GetUserAsync();
            if (bookingTrip.UserId != user.Id)
            {
                //  _logger.LogWarning("SubmitManualPaymentNotification: محاولة إشعار دفع يدوي لحجز {BookingId} لا يخص المستخدم {UserId}.", notificationDto.BookingId, userId);
                return Result.Failure("لا تملك صلاحية لتقديم إشعار دفع لهذا الحجز.", failureType: ResultFailureType.Forbidden);

            }

            // 3. التحقق من تاريخ الرحلة (لضمان عدم معالجة حجوزات مكتملة)
            if (
                bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Ongoing ||
                bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Completed
                )
            {
                //_logger.LogWarning("SubmitManualPaymentNotification: محاولة إشعار دفع يدوي للحجز {BookingId} لرحلة مكتملة بتاريخ {TripDate}.", notificationDto.BookingId, booking.TripDate.Date.ToShortDateString());
                return Result.BadRequest("Cann't Pay For a Trip That has Ongoin Or Completed ");
            }

            if (
                bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Cancelled)
            {
                //_logger.LogWarning("SubmitManualPaymentNotification: محاولة إشعار دفع يدوي للحجز {BookingId} لرحلة مكتملة بتاريخ {TripDate}.", notificationDto.BookingId, booking.TripDate.Date.ToShortDateString());
                return Result.BadRequest("This Trip Canselling By adming if you pay Sent Report");
            }

            if (bookingTrip.BookingStatus != BookingStatus.Pending || bookingTrip.Payment.PaymentStatus != PaymentStatus.Pending)
            {
                //TODO  Send email 
                await _notificationService.CreateInAppNotificationAsync(
                        bookingTrip.UserId,
                        "Booking Not Eligible For Payment",
                        $"Booking With id : {bookingTrip.Id} in {bookingTrip.BookingStatus.ToString()} Condition Or Payment in  {bookingTrip.Payment.PaymentStatus.ToString()} Condition. Cann't send a payment notice Please Booking again",
                        "BookingNotEligibleForPayment",
                        bookingTrip.Id.ToString());

                return Result.BadRequest($"Booking With id : {bookingTrip.Id} in {bookingTrip.BookingStatus.ToString()} Condition Or Payment in  {bookingTrip.Payment.PaymentStatus.ToString()} Condition. Cann't send a payment notice Please Booking again");
            }

            var existingPaymentWithSameTransaction = await _paymentRepositoryAsync.GetTableNoTracking()
                                                              .FirstOrDefaultAsync(p => p.TransactionRef == details.TransactionReference &&
                                                                                        p.PaymentMethodId == bookingTrip.Payment.PaymentMethodId);
            if (existingPaymentWithSameTransaction is not null)
            {
                await _notificationService.CreateInAppNotificationAsync(
                       bookingTrip.UserId,
                       "TransactionRef Used",
                       $"Transaction Reference id :  '{details.TransactionReference}' For BookingTrip id: {bookingTrip.Id} is Previoysly used",
                       "TransactionRefUsed",
                       bookingTrip.Id.ToString());

                return Result.BadRequest($"The Transaction With Id  : {details.TransactionReference} is Previosly Used ");

            }

            var payment = bookingTrip.Payment;

            if (!string.IsNullOrEmpty(payment.TransactionRef))
            {
                return Result.BadRequest(" Cann't send More Than one Notification Payment For the Same Booking");
            }

            payment.Amount = details.PaidAmount;                  // المبلغ المبلغ عنه
            payment.PaymentDate = details.PaymentDateTime;        // تاريخ الدفع المبلغ عنه
            payment.TransactionRef = details.TransactionReference; // رقم العملية الذي أرسله العميل

            await _paymentRepositoryAsync.UpdateAsync(payment);
            //_logger.LogInformation("SubmitManualPaymentNotification: تم إنشاء سجل دفعة يدوي جديد للحجز {BookingId}.", details.BookingId);

            // 7. إيقاف المؤقت إذا كان لا يزال يعمل
            _paymentTimerService.StopPaymentTimer(bookingTrip.Id);

            // 1.10. إرسال إشعار داخلي للعميل بأن إشعاره تم استلامه
            await _notificationService.CreateInAppNotificationAsync(
                bookingTrip.UserId,
                "إشعار دفعك قيد المراجعة",
                $"تم استلام إشعار الدفع الخاص بالحجز رقم {bookingTrip.Id} ({details.TransactionReference}). المبلغ: {details.PaidAmount:N2}. إجمالي المدفوع للحجز: {bookingTrip.ActualPrice:N2}. سيتم مراجعته قريباً.",
                "ManualPaymentReceived",
                bookingTrip.Id.ToString()
                );

            //_logger.LogInformation("SubmitManualPaymentNotification: تم تسجيل إشعار الدفع اليدوي بنجاح للحجز {BookingId}.", details.BookingId);
            return Result.Success("تم استلام إشعار الدفع اليدوي الخاص بك بنجاح. سيتم مراجعته قريباً.");

        }

        public async Task<Result> ProcessManualPaymentConfirmationAsync(ManualPaymentConfirmationRequestDto request)
        {
            var bookingTrip = await _bookingTripRepository.GetTableNoTracking()
                                                          .Include(b => b.Payment)
                                                          .Where(b => b.Payment.TransactionRef == request.TransactionRef)
                                                          .Include(b => b.Payment)
                                                          .FirstOrDefaultAsync();
            if (bookingTrip == null)
            {
                return Result.NotFound($"Not Found Booking Related With payment Have : {request.TransactionRef}");
            }


            if (bookingTrip.BookingStatus != BookingStatus.Pending)
            {
                return Result.BadRequest("The Booking is not Pending");
            }

            var paymentMethod = await _paymentMethodRepositoryAsync.GetTableNoTracking()
                                                                   .FirstOrDefaultAsync(pm => pm.Id == request.PaymentMethodId);
            if (paymentMethod == null)
            {
                return Result.NotFound($"Not Found Payment Method With id {request.PaymentMethodId}");
            }
            if (bookingTrip.Payment.PaymentMethodId != paymentMethod.Id)
            {
                return Result.BadRequest($"Payment Method For Booking With id {bookingTrip.Id} Does Not Match The Payment Method sent ");
            }
            using var transaction = await _bookingTripRepository.BeginTransactionAsync();
            try
            {
                if (!request.IsConfirmed)
                {
                    if (bookingTrip.ActualPrice < request.VerifiedAmount)
                    {
                        await transaction.RollbackAsync();
                        return Result.BadRequest($"you are refuse the pay ,but The Amount Verified is Greater than the amount for booking ,check again");
                    }
                    bookingTrip.BookingStatus = BookingStatus.Cancelled;
                    await _bookingTripRepository.UpdateAsync(bookingTrip);


                    var tripDate = await _packageTripDateRepository.GetByIdAsync(bookingTrip.PackageTripDateId);

                    if (tripDate != null)
                    {
                        tripDate.AvailableSeats += bookingTrip.PassengerCount;
                        await _packageTripDateRepository.UpdateAsync(tripDate);
                    }

                    bookingTrip.Payment.PaymentStatus = PaymentStatus.Cancelled;
                    await _paymentRepositoryAsync.UpdateAsync(bookingTrip.Payment);
                  
                    if (request.VerifiedAmount != 0)
                    {
                        var refunded = new Refund
                        {
                            AdminNotes = request.AdminNotes,
                            TransactionReference = bookingTrip.Payment.TransactionRef,
                            Amount = request.VerifiedAmount,
                            PaymentId = bookingTrip.Payment.Id,
                            CreatedAt = DateTime.UtcNow,
                            Status = RefundStatus.Pending
                        };

                        await _refundRepositoryAsync.AddAsync(refunded);
                        //TODO send email
                        await _notificationService.CreateInAppNotificationAsync(
                            bookingTrip.UserId,
                            "تم رفض طلب الدفع",
                            $"نعتذر، تم رفض طلب الدفع الخاص بالحجز رقم {bookingTrip.Id} ({bookingTrip.Payment.TransactionRef}) ." +
                            $"وتم الغاء الحجز" +
                            $" السبب: المبلغ الدفوع {request.VerifiedAmount} اقل من المبلغ الخاص بالحجز  {bookingTrip.ActualPrice} سوف يتم استرداد المبلغ المدفوع خلال 3-5 ايام عمل  ",
                            "BookingRejected"
                            );
                    }



                    await transaction.CommitAsync();

                    //logger.LogWarning("ManualConfirmation: الحجز {BookingId} تم رفض دفعه يدوياً. ملاحظات المسؤول: {Notes}", request.BookingId, request.AdminNotes);
                    return Result.Success("تم رفض الدفع اليدوي. الحجز ملغي.");
                }

                bookingTrip.BookingStatus = BookingStatus.Completed;
                await _bookingTripRepository.UpdateAsync(bookingTrip);

                bookingTrip.Payment.PaymentStatus = PaymentStatus.Completed;
                await _paymentRepositoryAsync.UpdateAsync(bookingTrip.Payment);
                await _notificationService.CreateInAppNotificationAsync(

                    bookingTrip.UserId,
                        "تم تاكيد طلب الدفع",
                        $" تم تاكيد طلب الدفع الخاص بالحجز رقم {bookingTrip.Id} ({bookingTrip.Payment.TransactionRef})." +
                        $"وتم تاكيد الحجز",
                        "BookingCompleted"
                        );
                if (request.VerifiedAmount > bookingTrip.ActualPrice)
                {
                    var refunded = new Refund
                    {
                        AdminNotes = request.AdminNotes,
                        Amount = request.VerifiedAmount - bookingTrip.ActualPrice,
                        PaymentId = bookingTrip.Payment.Id,
                        CreatedAt = DateTime.UtcNow,
                        Status = RefundStatus.Pending
                    };

                    await _refundRepositoryAsync.AddAsync(refunded);
                    await _notificationService.CreateInAppNotificationAsync(
                       bookingTrip.UserId,
                       "استرجاع مبلغ فائض ",
                       $"سوف يتم استرجاع المبلغ {refunded.Amount}  خلال 3-5 ايام عمل" +
                       $"استرجاع مبلغ فائض",
                       "BookingCompleted"
                       );
                }
                await transaction.CommitAsync();
                return Result.Success("تم تأكيد الدفع اليدوي بنجاح.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Failure("حدث خطا داخلي ");
            }

        }

        public async Task<Result> ReportMissingPaymentAsync(MissingPaymentReportDto reportDto)
        {
            var userId = _currentUserService.GetUserId();

            var existingReport = await _discrepancyReportRepositoryAsync.GetTableNoTracking()
                                          .FirstOrDefaultAsync(r => r.ReportedTransactionRef == reportDto.TransactionReference);

            if (existingReport != null)
            {
                if (existingReport.Status == PaymentDiscrepancyStatusEnum.PendingReview)
                    return Result.Failure("يوجد بلاغ سابق لنفس المشكلة قيد المراجعة.", failureType: ResultFailureType.Conflict);

                if (existingReport.Status == PaymentDiscrepancyStatusEnum.ReviewedConfirmed)
                    return Result.Failure("يوجد بلاغ سابق لنفس المشكلة تمت مراجعته.", failureType: ResultFailureType.Conflict);

                if (existingReport.Status == PaymentDiscrepancyStatusEnum.ReviewedRejected || existingReport.Status == PaymentDiscrepancyStatusEnum.Closed)
                    return Result.Failure("يوجد بلاغ سابق لنفس المشكلة تم رفضه .", failureType: ResultFailureType.Conflict);
            }

            var refunded = await _refundRepositoryAsync.GetTableNoTracking()
                                                       .Where(re => re.TransactionReference == reportDto.TransactionReference)
                                                       .FirstOrDefaultAsync();
            if (refunded is not null)
            {

                //ذا وصل لهون فاما قام العميل باسترجاع المبلغ من خلال الغاء حجز مكتمل  او قام ال
                //admin
                // بالغاء الرحلة فسوف يتم استرجاع المبلغ للمستخدمين
                // او الحجز الغي بسبب دفع مبلغ اقل من مبلغ الحجز 
                await _notificationService.CreateInAppNotificationAsync(
                    userId,
                    "بلاغ دفعك: الدفعة في حالة استرداد   ",
                    $"الدفعة برقم العملية '{reportDto.TransactionReference}' في حالة استرداد . لا حاجة لبلاغ جديد.",
                    "DiscrepancyReportPaymentRefunded",
                    reportDto.TransactionReference);
                return Result.Failure($"حالة استرجاع الرصيد لهذه الدفعة  {refunded.Status}.", failureType: ResultFailureType.Conflict);
            }

            var booking = await _bookingTripRepository.GetTableNoTracking()
                                                      .Include(b => b.Payment)
                                                      .Where(b => b.Payment.TransactionRef == reportDto.TransactionReference)
                                                      .Include(b => b.User)
                                                      .Include(b => b.PackageTripDate)
                                                      .FirstOrDefaultAsync();

            if (booking is not null)
            {
                if (booking.BookingStatus == BookingStatus.Pending)
                {
                    await _notificationService.CreateInAppNotificationAsync(userId,
                   "بلاغ دفعك: الدفعة في انتظار التحقق",
                   $"الدفعة برقم العملية '{reportDto.TransactionReference}' للحجز رقم {booking.Id} في انتظار التحقق. لا حاجة لبلاغ جديد.",
                   "DiscrepancyReportPaymentPending",
                    booking.Id.ToString());
                    return Result.BadRequest("Booking is pending");
                }
                if (booking.BookingStatus == BookingStatus.Completed)
                {
                    await _notificationService.CreateInAppNotificationAsync(userId,
                     "بلاغ دفعك: الدفعة مؤكدة  ",
                     $"الدفعة برقم العملية '{reportDto.TransactionReference}' للحجز رقم {booking.Id} مؤكدة  . لا حاجة لبلاغ جديد.",
                     "DiscrepancyReportPaymentConfirmed",
                     booking.Id.ToString());
                    return Result.BadRequest("الحجز لديه دفعة مؤكدة .");

                }
            }

            // 5. إنشاء سجل بلاغ جديد
            var newDiscrepancyReport = new PaymentDiscrepancyReport
            {
                UserId = userId,
                ReportedTransactionRef = reportDto.TransactionReference,
                ReportedPaymentDateTime = reportDto.PaymentDateTime,
                ReportedPaidAmount = reportDto.PaidAmount,
                CustomerNotes = reportDto.CustomerNotes,
                ReportDate = DateTime.UtcNow,
                Status = PaymentDiscrepancyStatusEnum.PendingReview,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _discrepancyReportRepositoryAsync.AddAsync(newDiscrepancyReport);

            //_logger.LogInformation("ReportPaymentDiscrepancy: تم تسجيل بلاغ جديد بنجاح للحجز {BookingId} بواسطة المستخدم {UserId}.", reportDto.BookingId, userId);

            // إرسال إشعار داخلي للعميل بتأكيد تسجيل بلاغه
            await _notificationService.CreateInAppNotificationAsync(
                userId,
                "بلاغ الدفع الخاص بك",
                $" : تم تسجيل بلاغك الخاص بالعملية رقم {reportDto.TransactionReference} . سيتم مراجعته قريباً.",
                "DiscrepancyReportSubmitted"
                );

            return Result.Success("تم تسجيل بلاغك بنجاح. سيتم مراجعته قريباً.");
        }

        public async Task<Result> ResolveMissingPaymentReportAsync(DiscrepancyReportProcessRequestDto discrepancyReport)
        {

            var report = await _discrepancyReportRepositoryAsync.GetTableNoTracking()
                                                                 .Where(r => r.Id == discrepancyReport.ReportId && r.Status == (int)PaymentDiscrepancyStatusEnum.PendingReview)
                                                                 .Include(r => r.Payment)
                                                                 .FirstOrDefaultAsync(r => r.Id == discrepancyReport.ReportId && r.Status == PaymentDiscrepancyStatusEnum.PendingReview);

            if (report == null)
            {
                //_logger.LogWarning("ProcessDiscrepancyReport: بلاغ التضارب {ReportId} غير موجود أو ليس في حالة 'بانتظار المراجعة'.", reportId);
                return Result.NotFound("بلاغ التضارب غير موجود أو ليس في حالة 'بانتظار المراجعة'.");
            }

            using var transaction = await _discrepancyReportRepositoryAsync.BeginTransactionAsync();
            try
            {
                // تحديث حالة البلاغ وملاحظات المسؤول
                report.Status = discrepancyReport.Status;
                report.AdminNotes = discrepancyReport.AdminNotes;
                report.ReviewedByUserId = _currentUserService.GetUserId(); ;
                report.ReviewDate = DateTime.UtcNow;
                await _discrepancyReportRepositoryAsync.UpdateAsync(report);


                //  _logger.LogInformation("ProcessDiscrepancyReport: تم تحديث بلاغ التضارب {ReportId} إلى حالة {Status} بنجاح.", reportId, status);

                // إشعار العميل بحالة البلاغ (وليس حالة الدفع)
                string notificationTitle = "";
                string notificationMessage = "";
                string notificationType = "";

                if (discrepancyReport.Status == PaymentDiscrepancyStatusEnum.ReviewedConfirmed)
                {
                    notificationTitle = "تم مراجعة بلاغ الدفع";
                    notificationMessage = $"تم مراجعة بلاغك بخصوص الدفعة رقم {report.ReportedTransactionRef} تم تأكيد المراجعة.";
                    notificationType = "ReportReviewedConfirmed";



                    var refund = new Refund()
                    {
                        AdminNotes = discrepancyReport.AdminNotes ?? "",
                        ProcessedByUserId = _currentUserService.GetUserId(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Amount = discrepancyReport.VerifiedAmount,
                        TransactionReference = report.ReportedTransactionRef,
                        TransactionRefunded = string.Empty,
                        ReportId = report.Id,

                    };
                    await _refundRepositoryAsync.AddAsync(refund);
                }
                else if (discrepancyReport.Status == PaymentDiscrepancyStatusEnum.ReviewedRejected)
                {
                    notificationTitle = "تم رفض بلاغ الدفع";
                    notificationMessage = $"تم مراجعة بلاغك بخصوص الدفعة رقم {report.ReportedTransactionRef}  " +
                        $" وتم رفضه. السبب: {discrepancyReport.AdminNotes}. يرجى التواصل مع الدعم الفني.";
                    notificationType = "ReportReviewedRejected";
                }

                if (!string.IsNullOrEmpty(notificationTitle))
                {
                    await _notificationService.CreateInAppNotificationAsync(report.UserId, notificationTitle, notificationMessage, notificationType);
                }
                await transaction.CommitAsync();
                return Result.Success($"تم معالجة بلاغ التضارب بنجاح إلى حالة {discrepancyReport.Status}.");

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }


        }

        public async Task<Result<PaymentTransactionStatusDto>> GetDetailsTransactionAsync(string transactionReference)
        {
            // سننشئ DTO جديد لتلخيص الحالة
            var transactionStatus = new PaymentTransactionStatusDto
            {
                TransactionReference = transactionReference,
                ExistsInSystem = false
            };

            // 1. البحث عن الدفعة (Payment) بهذا TransactionReference
            var payment = await _paymentRepositoryAsync.GetTableNoTracking()
                .Where(p => p.TransactionRef == transactionReference)
                .Include(p => p.BookingTrip)
                .ThenInclude(b => b.PackageTripDate)
                .Include(p => p.BookingTrip)
                .ThenInclude(b => b.User)
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync();

            if (payment != null)
            {
                var PaymentDetailDto = new PaymentDetailDto
                {
                    PaymentId = payment.Id,
                    PaymentAmount = payment.Amount,
                    PaymentVerifiedAmount = payment.Amount,
                    PaymentStatus = payment.PaymentStatus,
                    PaymentDate = payment.PaymentDate,
                    PaymentMethodName = payment.PaymentMethod?.Name
                };
                transactionStatus.PaymentDetailDto = PaymentDetailDto;
                var BookingTripDetailDto = new BookingTripDetailDto
                {
                    AssociatedBookingId = payment.BookingTripId,
                    BookingStatus = payment.BookingTrip.BookingStatus,
                    TripDate = payment.BookingTrip.PackageTripDate?.StartPackageTripDate,
                    TripDateStatus = payment.BookingTrip.PackageTripDate != null ? payment.BookingTrip.PackageTripDate.Status : null
                };
                transactionStatus.BookingTripDetailDto = BookingTripDetailDto;

            }

            // 2. البحث عن البلاغات (PaymentDiscrepancyReport) بهذا TransactionReference
            var discrepancyReport = await _discrepancyReportRepositoryAsync.GetTableNoTracking()
                .Where(r => r.ReportedTransactionRef == transactionReference)
                .FirstOrDefaultAsync();


            if (discrepancyReport != null)
            {
                var ReportDetailDto = new ReportDetailDto
                {
                    ReportId = discrepancyReport.Id,
                    ReportStatus = discrepancyReport.Status,
                    ReportedAmount = discrepancyReport.ReportedPaidAmount,
                    ReportedPaymentDateTime = discrepancyReport.ReportedPaymentDateTime,
                    CustomerNotes = discrepancyReport.CustomerNotes,
                    AdminNotesOnReport = discrepancyReport.AdminNotes,
                };
                transactionStatus.ReportDetailDto = ReportDetailDto;

                // إذا لم يكن هناك payment، لكن هناك report، هذا يعني دفعة يتيمة غير معروفة للنظام
                if (payment == null)
                {
                    //_logger.LogWarning("VerifyPaymentTransaction: تم العثور على بلاغ {ReportId} بدون سجل دفع مطابق لمعرف العملية {TransactionRef}.", discrepancyReport.Id, transactionReference);
                    transactionStatus.DiscrepancyType = "بلاغ بدون دفعة مطابقة";
                }
            }

            // 3. البحث عن سجلات الاسترداد (Refunds) المرتبطة بهذا TransactionReference (لو تم الاسترداد)
            var refund = await _refundRepositoryAsync.GetTableNoTracking()
                                                     .Where(r => r.TransactionReference == transactionReference) // TransactionReference في Refund هو لمعرف عملية الاسترداد
                                                     .FirstOrDefaultAsync();

            if (refund != null)
            {
                var RefundedDetailDto = new RefundedDetailDto
                {
                    RefundId = refund.Id,
                    RefundAmount = refund.Amount,
                    RefundStatus = refund.Status,
                    RefundProcessedDate = refund.RefundProcessedDate,
                    AdminNotesOnRefund = refund.AdminNotes,
                };
                transactionStatus.RefundedDetailDto = RefundedDetailDto;

            }

            if (payment == null) // لو في Refund بس ما في Payment (سيناريو غريب، ممكن يكون Refund لدفعة مجهولة)
            {
                transactionStatus.DiscrepancyType = "استرداد بدون دفعة أصلية";
            }
            else if (payment != null && payment.PaymentStatus == PaymentStatus.Refunded)
            {
                transactionStatus.DiscrepancyType = "دفعة مستردة";
            }
            else if (payment != null && payment.PaymentStatus == PaymentStatus.Pending && payment.BookingTrip.BookingStatus == BookingStatus.Pending)
            {
                transactionStatus.DiscrepancyType = "دفعة معلقة بانتظار التأكيد";
            }
            else if (payment != null && payment.PaymentStatus == PaymentStatus.Cancelled)
            {
                transactionStatus.DiscrepancyType = "دفعة  ملغاة";
            }
            else if (payment != null && payment.Amount != payment.BookingTrip?.ActualPrice)
            {
                transactionStatus.DiscrepancyType = "مبلغ غير مطابق";
            }

            // إذا لم يتم العثور على أي شيء، هذا الرقم غير معروف للنظام
            if (payment is not null || refund is not null)
            {
                transactionStatus.ExistsInSystem = true;
            }
            if (!transactionStatus.ExistsInSystem && discrepancyReport is null)
            {
                return Result<PaymentTransactionStatusDto>.NotFound("Not Found Transaction");
            }
            return Result<PaymentTransactionStatusDto>.Success(transactionStatus);
        }


        // 11. جلب بلاغات الدفع المفقودة للمدير
        public async Task<Result<IEnumerable<MissingPaymentReportResponceDto>>> GetMissingPaymentReportsForAdminAsync()
        {
            // _logger.LogInformation("GetMissingPaymentReports: جلب بلاغات الدفع المفقودة للمدير.");
            var pendingReports = await _discrepancyReportRepositoryAsync.GetTableNoTracking()
                                                                        .Where(r => r.Status == PaymentDiscrepancyStatusEnum.PendingReview)
                                                                        .ToListAsync();

            var resultDtos = pendingReports.Select(r => new MissingPaymentReportResponceDto // نستخدم نفس DTO للعرض
            {
                ReportId = r.Id,
                TransactionReference = r.ReportedTransactionRef,
                PaymentDateTime = r.ReportedPaymentDateTime,
                PaidAmount = r.ReportedPaidAmount,
                CustomerNotes = r.CustomerNotes
            }).ToList();

            if (!resultDtos.Any())
            {
                return Result<IEnumerable<MissingPaymentReportResponceDto>>.NotFound("Not Found any Report Pending");
            }

            return Result<IEnumerable<MissingPaymentReportResponceDto>>.Success(resultDtos);
        }

        public async Task<Result<IEnumerable<ManualPaymentDetailsDto>>> GetPendingManualPaymentsForAdminAsync()
        {
            var paymnetsPending = await _paymentRepositoryAsync.GetTableNoTracking()
                                                               .Where(p => p.PaymentStatus == PaymentStatus.Pending)
                                                               .Include(p => p.PaymentMethod)
                                                               .ToListAsync();

            paymnetsPending = paymnetsPending.Where(p => !string.IsNullOrEmpty(p.TransactionRef)).ToList();
            if (!paymnetsPending.Any())
            {
                return Result<IEnumerable<ManualPaymentDetailsDto>>.NotFound("Not Found Any payment Pending");
            }

            var manualPaymentDetailsDtos = new List<ManualPaymentDetailsDto>();
            foreach (var payment in paymnetsPending)
            {
                manualPaymentDetailsDtos.Add(new ManualPaymentDetailsDto
                {
                    BookingId = payment.BookingTripId,
                    PaidAmount = payment.Amount,
                    PaymentDateTime = payment.PaymentDate,
                    PaymentMethodId = payment.PaymentMethodId,
                    PaymentMethodName = payment.PaymentMethod.Name,
                    TransactionReference = payment.TransactionRef?? string.Empty

                });
            }
            return Result<IEnumerable<ManualPaymentDetailsDto>>.Success(manualPaymentDetailsDtos);


        }

        public async Task<Result<IEnumerable<PaymentMethodDto>>> GetPaymentMethods()
        {
            var paymentMethods = await _paymentMethodRepositoryAsync.GetTableNoTracking()
                                                                    .ToListAsync();
            if (paymentMethods is null)
                return Result<IEnumerable<PaymentMethodDto>>.NotFound("not Found Payment Method");
            var paymentMethodsResult = paymentMethods.Select(x => new PaymentMethodDto
            {
                Id = x.Id,
                Name = x.Name
            });
            return Result<IEnumerable<PaymentMethodDto>>.Success(paymentMethodsResult);
        }
    }
}



