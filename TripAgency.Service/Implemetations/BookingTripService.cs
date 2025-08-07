using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using TripAgency.Data.Entities;
using TripAgency.Data.Enums;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.BookingTrip.Commands;
using TripAgency.Service.Feature.BookingTrip.Queries;
using TripAgency.Service.Feature.Payment;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class BookingTripService : GenericService<BookingTrip, GetBookingTripByIdDto, GetBookingTripsDto, AddBookingTripDto, UpdateBookingTripDto>, IBookingTripService
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
        public IPaymentService _paymentService { get; }
        public IConfiguration _configuration { get; }
        public IMapper _mapper { get; }
        private string baseUrl;
        public BookingTripService(IBookingTripRepositoryAsync bookingTripRepository,
                                  IPackageTripDateRepositoryAsync tripDateRepository,
                                  IPaymentMethodRepositoryAsync paymentMethodRepositoryAsync,
                                  IPaymentTimerService paymentTimerService,
                                  IPaymentGatewayFactory paymentGatewayFactory,
                                  INotificationService notificationService,
                                  IPaymentDiscrepancyReportRepositoryAsync discrepancyReportRepositoryAsync,
                                  IPaymentRepositoryAsync paymentRepositoryAsync,
                                  IPaymentService paymentService,
                                  IRefundRepositoryAsync refundRepositoryAsync,
                                  IMapper mapper,
                                  IConfiguration configuration
                                 ) : base(bookingTripRepository, mapper)
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
            _paymentService = paymentService;
            baseUrl = configuration["BaseUrl"] ?? throw new InvalidOperationException("Not Found BaseUrl."); ;
        }
        public async Task<Result<PaymentInitiationResponseDto>> InitiateBookingAndPaymentAsync(AddBookingPackageTripDto bookPackageDto)
        {
            // 1.1. التحقق من توفر المقاعد وتاريخ الرحلة
            var packageTripDate = await _packageTripDateRepository.GetTableNoTracking()
                                                                  .Where(pt => pt.Id == bookPackageDto.PackageTripDateId)
                                                                  .Include(pt => pt.PackageTrip)
                                                                  .ThenInclude(pt => pt.PackageTripDestinations)
                                                                  .ThenInclude(pt => pt.PackageTripDestinationActivities)
                                                                  .FirstOrDefaultAsync();
            if (packageTripDate is null)
            {
                return Result<PaymentInitiationResponseDto>.NotFound($"Not Found PackageTripDate With Id : {bookPackageDto.PackageTripDateId}");
            }
            if (packageTripDate.AvailableSeats < bookPackageDto.PassengerCount)
            {
                return Result<PaymentInitiationResponseDto>.BadRequest($"No Available seats at this date ");
            }
            if (packageTripDate.Status != PackageTripDateStatus.Published)
            {
                return Result<PaymentInitiationResponseDto>.BadRequest($"The PackageTrip Not Available");
            }

            // 1.2. جلب طريقة الدفع المختارة
            var paymentMethod = await _paymentMethodRepositoryAsync.GetTableNoTracking()
                                                                   .FirstOrDefaultAsync(pm => pm.Id == bookPackageDto.PaymentMethodId);
            if (paymentMethod is null)
            {
                //  _logger.LogWarning("InitiatePayment: طريقة الدفع المختارة غير صالحة: {MethodName}", bookPackageDto.PaymentMethodName);
                return Result<PaymentInitiationResponseDto>.BadRequest("Invalid Payment method .");
            }

            // النحقق من السعر الفعلي 
            var calculatedTotalPrice = packageTripDate.PackageTrip.Price + packageTripDate.PackageTrip.PackageTripDestinations.Sum(ptd => ptd.PackageTripDestinationActivities.Sum(ptda => ptda.Price));
            if (calculatedTotalPrice != bookPackageDto.AmountPrice)
            {
                return Result<PaymentInitiationResponseDto>.BadRequest($"The ActualPrice is {calculatedTotalPrice}");
            }
            using var Transaction = await _bookingTripRepository.BeginTransactionAsync();
            try
            {
                // 1.3. إنشاء BookingTrip في DB بحالة 'معلّق' (Pending)
                var bookingTrip = new BookingTrip
                {
                    PackageTripDateId = packageTripDate.Id,
                    PassengerCount = bookPackageDto.PassengerCount,
                    BookingDate = DateTime.UtcNow,
                    BookingStatus = BookingStatus.Pending,
                    ActualPrice = calculatedTotalPrice,
                    Notes = $"Pending payment via {paymentMethod.Name}",
                    UserId = 2 // TODO
                };

                await _bookingTripRepository.AddAsync(bookingTrip);

                // 1.4. نقصان المقاعد المتاحة (وحفظ)
                packageTripDate.AvailableSeats -= bookPackageDto.PassengerCount;
                await _packageTripDateRepository.UpdateAsync(packageTripDate);

                // 1.5. جلب خدمة بوابة الدفع المناسبة من المصنع
                var gatewayService = _paymentGatewayFactory.GetGatewayService(paymentMethod.Name);

                // 1.6. بناء طلب الدفع
                //var user =new User();
                //if (user == null)
                //{
                //    // _logger.LogError("InitiatePayment: بيانات المستخدم غير متوفرة للحجز {BookingId}.", booking.Id);
                //    // نرجع المقاعد ونلغي الحجز المؤقت
                //    booking.BookingStatus = BookingStatus.Cancelled;
                //    await _bookingRepository.UpdateAsync(booking);
                //    packageTripDate.AvailableSeats += bookPackageDto.PassengerCount;
                //    await _packageTripDateRepository.UpdateAsync(packageTripDate);
                //    await _bookingRepository.SaveChangesAsync();
                //    return Result<PaymentInitiationResponseDto>.Failure("بيانات المستخدم غير متوفرة لتهيئة الدفع.");
                //}


                //TODO
                var paymentRequest = new PaymentRequest
                {
                    BookingId = bookingTrip.Id,
                    Amount = bookPackageDto.AmountPrice,
                    Currency = "USD",
                    //CustomerEmail = user.Email,
                    SuccessCallbackUrl = $"{baseUrl}/payment/success?bookingId={bookingTrip.Id}&method={paymentMethod.Name}",
                    FailureCallbackUrl = $"{baseUrl}/payment/failure?bookingId={bookingTrip.Id}&method={paymentMethod.Name}",
                    CancelCallbackUrl = $"{baseUrl}/payment/cancel?bookingId={bookingTrip.Id}&method={paymentMethod.Name}"
                };

                var initiationResult = await gatewayService.InitiatePaymentAsync(paymentRequest);

                // 1.9. إنشاء سجل Payment مبدئي (دفعة معلقة)
                // هذا السجل يُنشأ دائمًا هنا، ويتم تحديثه لاحقًا بواسطة الـ callback أو التأكيد اليدوي
                var payment = new Payment
                {
                    BookingTripId = bookingTrip.Id,
                    PaymentMethodId = paymentMethod.Id,
                    Amount = calculatedTotalPrice, // المبلغ المراد دفعه
                    PaymentDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Pending, // الحالة الأولية للدفع
                    TransactionId = string.Empty
                };
                await _paymentRepositoryAsync.AddAsync(payment);

                if (!initiationResult.IsSuccess)
                {
                    // _logger.LogError("InitiatePayment: فشل تهيئة الدفع للحجز {BookingId} عبر البوابة {Gateway}: {Error}", booking.Id, selectedPaymentMethod.Name, initiationResult.Message);
                    // نرجع المقاعد ونلغي الحجز المؤقت
                    await Transaction.RollbackAsync();
                    return Result<PaymentInitiationResponseDto>.Failure($"Failed Initiate payment: {initiationResult.Message}");
                }

                // 1.7. بدء المؤقت الزمني 15 دقيقة
                var responseDto = initiationResult.Value!;
                responseDto.BookingTripId = bookingTrip.Id; // نتأكد إن الـ BookingId موجود
                responseDto.ExpireTime = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("ExpireBookingTime"));
                _paymentTimerService.StartPaymentTimer(bookingTrip.Id, TimeSpan.FromMinutes(_configuration.GetValue<int>("ExpireBookingTime")));

                // 
                await Transaction.CommitAsync();
                return Result<PaymentInitiationResponseDto>.Success(responseDto);
            }
            catch (Exception)
            {
                await Transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<Result> CancellingBookingAndRefundPayemntAsync(int bookingId)
        {
            var bookingTrip = await _bookingTripRepository.GetTableNoTracking()
                                                      .Where(b => b.Id == bookingId)
                                                      .Include(b => b.Payment)
                                                      .Include(b => b.PackageTripDate)
                                                      .FirstOrDefaultAsync();
            if (bookingTrip is null)
            {
                return Result.NotFound($"Not Found Booking With Id : {bookingId}");
            }
          
            if (bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Cancelled)
            {
                return Result.BadRequest("Cann't Cancelling Booking For PackageTrip Cancelled");
            }

            if (bookingTrip.BookingStatus == BookingStatus.Pending)
            {
               
                if (string.IsNullOrEmpty(bookingTrip.Payment.TransactionId))
                { 
                     _paymentTimerService.StopPaymentTimer(bookingId);
                    await _paymentService.HandlePaymentTimeoutAsync(bookingId);
                    await _notificationService.CreateInAppNotificationAsync(
                        bookingTrip.UserId,
                        "تم الغاء طلب الحجز بنجاح",
                        $" تم الغاء طلب الججز {bookingTrip.Id} ." +
                        $" السبب: طلب المستخدم الغاء الحجز  ",
                        "BookingCancelling",
                        bookingId.ToString());
                    // TODO Send Email 
                    return Result.Success();
                }
                else
                {
                    await _paymentService.HandlePaymentTimeoutAsync(bookingId);
                   
                    var newDiscrepancyReport = new PaymentDiscrepancyReport
                    {
                        UserId = 2 ,//TODO
                        ReportedTransactionRef = bookingTrip.Payment.TransactionId,
                        ReportedPaymentDateTime = bookingTrip.Payment.PaymentDate,
                        ReportedPaidAmount = bookingTrip.Payment.Amount,
                        CustomerNotes = "تم انشاء هذا التقرير من قبل النظام عندما الغى المستخدم الحجز",
                        ReportDate = DateTime.UtcNow,
                        Status = PaymentDiscrepancyStatusEnum.PendingReview,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _discrepancyReportRepositoryAsync.AddAsync(newDiscrepancyReport);

                    await _notificationService.CreateInAppNotificationAsync(
                        bookingTrip.UserId,
                        "تم الغاء طلب الحجز بنجاح",
                        $" تم الغاء طلب الججز {bookingTrip.Id} ." +
                        $"   : السبب: طلب المستخدم الغاء الحجز سوف يتم التحقق من رقم المعاملة {bookingTrip.Payment.TransactionId} واجراء التدابير المناسبة  ",
                        "BookingCancelling",
                        bookingId.ToString());
                    return Result.Success();
                }
            }           
            if (bookingTrip.BookingStatus == BookingStatus.Completed)
            {
                //TODO
                //CHECK time cancelling 
                //check status Trip 
                //Cancelling Boooking 
                //Refund Payment 
                if(bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Ongoing ||
                   bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Completed)
                {
                    return Result.BadRequest("Cann't Cancelling Booking For PackageTrip Ongoing Or  Completed ");
                }
               
                bookingTrip.BookingStatus = BookingStatus.Cancelled;
                await _bookingTripRepository.UpdateAsync(bookingTrip);

                var tripDate = await _packageTripDateRepository.GetByIdAsync(bookingTrip.PackageTripDateId);
                if (tripDate != null)
                {
                    tripDate.AvailableSeats += bookingTrip.PassengerCount;
                    await _packageTripDateRepository.UpdateAsync(tripDate);
                }

                //Refunded Payment
                var refunded = new Refund
                {
                    UpdatedAt = DateTime.Now,
                    Status = RefundStatus.Pending,
                    TransactionReference = bookingTrip.Payment.TransactionId,
                    CreatedAt = DateTime.Now,
                    PaymentId = bookingTrip.Payment.Id,
                    ProcessedByUserId = 2,
                    AdminNotes = "Refunded Because User Cancelling Booking"


                };
                await _refundRepositoryAsync.AddAsync(refunded);

                bookingTrip.Payment.PaymentStatus = PaymentStatus.Cancelled; //TODO refund or cancelling
                await _paymentRepositoryAsync.UpdateAsync(bookingTrip.Payment);

                return Result.Success();
            }

            
            return Result.BadRequest("Cann't Cancelling BookingTrip Cancelled Before");
        }
    }
}



