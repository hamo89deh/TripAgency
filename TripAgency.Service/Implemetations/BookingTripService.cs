using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        public ICurrentUserService _currentUserService { get; }
        public IOfferService _offerService { get; }
        public IPackageTripOffersService _packageTripOffersService { get; }
        public ILogger<BookingTripService> _logger { get; }
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
                                  IConfiguration configuration,
                                  ICurrentUserService currentUserService,
                                  IOfferService promotionService,
                                  IPackageTripOffersService packageTripOffersService,
                                  ILogger<BookingTripService> logger
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
            _currentUserService = currentUserService;
            _offerService = promotionService;
            _packageTripOffersService = packageTripOffersService;
            _logger = logger;
            _paymentService = paymentService;
            baseUrl = configuration["BaseUrl"] ?? throw new InvalidOperationException("Not Found BaseUrl."); ;
        }
        public async Task<Result<PaymentInitiationResponseDto>> InitiateBookingAndPaymentAsync(AddBookingPackageTripDto bookPackageDto)
        {
            var user = await _currentUserService.GetUserAsync();

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
            // 1.4. استرجاع العرض الترويجي الصالح
            var promotionResult = await _packageTripOffersService.GetValidOfferAsync(packageTripDate.PackageTripId);
            int? appliedPromotionId = null;
            decimal finalPrice = calculatedTotalPrice * bookPackageDto.PassengerCount;

            if (promotionResult.IsSuccess && promotionResult.Value != null)
            {
                var promotion = promotionResult.Value;
                appliedPromotionId = promotion.Id;
                finalPrice = calculatedTotalPrice * (1 - promotion.DiscountPercentage / 100);
                _logger.LogInformation("Applied promotion with Id: {PromotionId} with DiscountPercentage: {DiscountPercentage}% for PackageTripId: {PackageTripId}. FinalPrice: {FinalPrice}",
                    promotion.Id, promotion.DiscountPercentage, packageTripDate.PackageTripId, finalPrice);
            }
            else
            {
                _logger.LogInformation("No valid promotion found for PackageTripId: {PackageTripId}. Using total price: {TotalPrice}",
                    packageTripDate.PackageTripId, calculatedTotalPrice);
            }

            // 1.5. التحقق من السعر المقدم من العميل
            if (finalPrice != bookPackageDto.AmountPrice)
            {
                return Result<PaymentInitiationResponseDto>.BadRequest($"The ActualPrice is {finalPrice}");
            }
            using var Transaction = await _bookingTripRepository.BeginTransactionAsync();
            try
            {
                // 1.3. إنشاء BookingTrip في DB بحالة 'معلّق' (Pending)
                var bookingTrip = new BookingTrip
                {
                    PackageTripDateId = packageTripDate.Id,
                    PassengerCount = bookPackageDto.PassengerCount,
                    BookingDate = DateTime.Now,
                    BookingStatus = BookingStatus.Pending,
                    AppliedOfferId = appliedPromotionId,
                    ActualPrice = finalPrice,
                    Notes = bookPackageDto.Notes ?? string.Empty,
                    UserId = user.Id,
                    ExpireTime = DateTime.Now.AddMinutes(_configuration.GetValue<int>("ExpireBookingTime"))

                };

                await _bookingTripRepository.AddAsync(bookingTrip);

                // 1.4. نقصان المقاعد المتاحة (وحفظ)
                packageTripDate.AvailableSeats -= bookPackageDto.PassengerCount;
                if (packageTripDate.AvailableSeats == 0)
                    packageTripDate.Status = PackageTripDateStatus.Full;
                await _packageTripDateRepository.UpdateAsync(packageTripDate);

                // 1.5. جلب خدمة بوابة الدفع المناسبة من المصنع
                var gatewayService = _paymentGatewayFactory.GetGatewayService(paymentMethod.Name);

                var paymentRequest = new PaymentRequest
                {
                    BookingId = bookingTrip.Id,
                    Amount = finalPrice,
                    Currency = "USD",
                    CustomerEmail = user.Email!,
                    SuccessCallbackUrl = $"{baseUrl}/payment/success?bookingId={bookingTrip.Id}&method={paymentMethod.Name}",
                    FailureCallbackUrl = $"{baseUrl}/payment/failure?bookingId={bookingTrip.Id}&method={paymentMethod.Name}",
                    CancelCallbackUrl = $"{baseUrl}/payment/cancel?bookingId={bookingTrip.Id}&method={paymentMethod.Name}"
                };

                var initiationResult = await gatewayService.InitiatePaymentAsync(paymentRequest);

              
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
                responseDto.ExpireTime = DateTime.Now.AddMinutes(_configuration.GetValue<int>("ExpireBookingTime"));

                // 1.9. إنشاء سجل Payment مبدئي (دفعة معلقة)
                // هذا السجل يُنشأ دائمًا هنا، ويتم تحديثه لاحقًا بواسطة الـ callback أو التأكيد اليدوي
                var payment = new Payment
                {
                    BookingTripId = bookingTrip.Id,
                    PaymentMethodId = paymentMethod.Id,
                    Amount = finalPrice, // المبلغ المراد دفعه
                    PaymentDate = DateTime.Now,
                    PaymentStatus = PaymentStatus.Pending, // الحالة الأولية للدفع
                    TransactionRef = string.Empty,
                    PaymentInstructions = responseDto.PaymentInstructions
                };
                await _paymentRepositoryAsync.AddAsync(payment);              
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
            var user = await _currentUserService.GetUserAsync();
            using var transaction = await _bookingTripRepository.BeginTransactionAsync();
            try
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

                    if (string.IsNullOrEmpty(bookingTrip.Payment.TransactionRef))
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
                        await transaction.CommitAsync();
                        return Result.Success("successfully Canceled");
                    }
                    else
                    {
                        await _paymentService.HandlePaymentTimeoutAsync(bookingId);

                        var newDiscrepancyReport = new PaymentDiscrepancyReport
                        {
                            UserId = user.Id,
                            ReportedTransactionRef = bookingTrip.Payment.TransactionRef,
                            ReportedPaymentDateTime = bookingTrip.Payment.PaymentDate,
                            ReportedPaidAmount = bookingTrip.Payment.Amount,
                            CustomerNotes = "تم انشاء هذا التقرير من قبل النظام عندما الغى المستخدم الحجز",
                            ReportDate = DateTime.Now,
                            Status = PaymentDiscrepancyStatusEnum.PendingReview,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        await _discrepancyReportRepositoryAsync.AddAsync(newDiscrepancyReport);

                        await _notificationService.CreateInAppNotificationAsync(
                            bookingTrip.UserId,
                            "تم الغاء طلب الحجز بنجاح",
                            $" تم الغاء طلب الججز {bookingTrip.Id} ." +
                            $"   : السبب: طلب المستخدم الغاء الحجز سوف يتم التحقق من رقم المعاملة {bookingTrip.Payment.TransactionRef} واجراء التدابير المناسبة  ",
                            "BookingCancelling",
                            bookingId.ToString());
                        await transaction.CommitAsync();
                        return Result.Success("successfully Canceled Wait the admin to verify the validity of the payment");
                    }
                }
                if (bookingTrip.BookingStatus == BookingStatus.Completed)
                {
                    //TODO  Chck time cancelling to decide how must refund

                    if (bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Ongoing ||
                       bookingTrip.PackageTripDate.Status == PackageTripDateStatus.BookingClosed ||
                       bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Completed ||
                       bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Cancelled)
                    {
                        return Result.BadRequest("Cann't Cancelling Booking For PackageTrip Ongoing Or Completed Or BookingClosed");
                    }

                    bookingTrip.BookingStatus = BookingStatus.Cancelled;
                    await _bookingTripRepository.UpdateAsync(bookingTrip);

                    bookingTrip.PackageTripDate.AvailableSeats += bookingTrip.PassengerCount;

                    if (bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Full)
                    {
                        if (DateTime.Now.Date <= bookingTrip.PackageTripDate.EndBookingDate.Date)
                            bookingTrip.PackageTripDate.Status = PackageTripDateStatus.Published;
                        else
                            bookingTrip.PackageTripDate.Status = PackageTripDateStatus.BookingClosed;
                    }
                    await _packageTripDateRepository.UpdateAsync(bookingTrip.PackageTripDate);

                    bookingTrip.Payment.PaymentStatus = PaymentStatus.Cancelled;
                    await _paymentRepositoryAsync.UpdateAsync(bookingTrip.Payment);
                    //Refunded Payment
                    var refunded = new Refund
                    {
                        Status = RefundStatus.Pending,
                        TransactionReference = bookingTrip.Payment.TransactionRef ?? string.Empty,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Amount = bookingTrip.Payment.Amount,
                        PaymentId = bookingTrip.Payment.Id
                    };
                    await _refundRepositoryAsync.AddAsync(refunded);
                    await transaction.CommitAsync();
                    return Result.Success("successfully Canceled,Paymnets will be refunded within Three working days");
                }


                return Result.BadRequest("Cann't Cancelling BookingTrip Cancelled Before");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error when Cancelled user Booking the message : {ex.Message}");
                return Result.Failure("Error in the system Try later"); ;
            }
        }

        public async Task<Result<IEnumerable<GetBookingTripForUserDto>>> GetBookingsPackageTripForUserAsync(BookingStatus bookingStatus)
        {
            var user = await _currentUserService.GetUserAsync();
            var bookingTrips = await _bookingTripRepository.GetTableNoTracking()
                                                      .Where(x => x.UserId == user.Id && x.BookingStatus == bookingStatus)
                                                      .Include(x => x.Payment)
                                                      .ThenInclude(x=>x.PaymentMethod)
                                                      .Include(x=>x.PackageTripDate)
                                                      .ThenInclude(x=>x.PackageTrip)
                                                      .ToListAsync();
            if(bookingTrips.Count()==0)
                return Result<IEnumerable<GetBookingTripForUserDto>>.NotFound($"Not Found any Booking {bookingStatus}");


            var result = bookingTrips.Select(x => new GetBookingTripForUserDto()
            {
                BookingId = x.Id,
                ActualPrice = x.ActualPrice,
                BookingDate = x.BookingDate,
                BookingStatus = bookingStatus,
                Notes = x.Notes,
                PassengerCount = x.PassengerCount,
                GetPackageTripDateBookingDetailDto = new GetPackageTripDateBookingDetailDto
                {
                    StartTripDate = x.PackageTripDate.StartPackageTripDate,
                    EndTripDate=x.PackageTripDate.EndPackageTripDate,
                    PackageTripDateStatus =x.PackageTripDate.Status,
                    PackageTripdateId =x.PackageTripDateId,
                    ImageUrlPackageTrip=x.PackageTripDate.PackageTrip.ImageUrl,
                    PackageTripName =x.PackageTripDate.PackageTrip.Name ,
                    StartBookingTripDate = x.PackageTripDate.StartBookingDate,
                    EndBookingTripDate=x.PackageTripDate.EndBookingDate   ,
                    CanReviewTrip = x.PackageTripDate.Status == PackageTripDateStatus.Completed && x.BookingStatus == BookingStatus.Completed
                } ,
                UserId = user.Id,
                ExpireTime = x.ExpireTime,
                GetPaymentDto = new GetPaymentDto()
                {
                    PaymentId = x.Id,
                    Amount = x.Payment.Amount,
                    PaymentDate = x.Payment.PaymentDate,
                    PaymentMethodName = x.Payment.PaymentMethod.Name,
                    PaymentStatus = x.Payment.PaymentStatus,
                    TransactionRef = x.Payment.TransactionRef ?? string.Empty,
                    PaymentInstructions= x.Payment.PaymentInstructions ?? string.Empty,
                    CanCompletePayment =string.IsNullOrEmpty(x.Payment.TransactionRef) && x.Payment.PaymentStatus == PaymentStatus.Pending

                   
                }
            });
            return Result<IEnumerable<GetBookingTripForUserDto>>.Success(result);
        }

        public async Task<Result<GetBookingTripForUserDto>> GetBookingPackageTripForUserAsync(int bookingId, BookingStatus bookingStatus)
        {
            var user = await _currentUserService.GetUserAsync();
            var bookingTrip = await _bookingTripRepository.GetTableNoTracking()
                                                      .Where(x => x.UserId == user.Id && x.BookingStatus == bookingStatus)
                                                      .Include(x => x.Payment)
                                                      .ThenInclude(x => x.PaymentMethod)
                                                      .Include(x => x.PackageTripDate)
                                                      .ThenInclude(x => x.PackageTrip)
                                                      .FirstOrDefaultAsync();
            if (bookingTrip is null)
                return Result<GetBookingTripForUserDto>.NotFound($"Not Found any Booking {bookingStatus}");
            var result = new GetBookingTripForUserDto()
            {
                BookingId = bookingTrip.Id,
                ActualPrice = bookingTrip.ActualPrice,
                BookingDate = bookingTrip.BookingDate,
                BookingStatus = bookingStatus,
                Notes = bookingTrip.Notes,
                PassengerCount = bookingTrip.PassengerCount,
                GetPackageTripDateBookingDetailDto = new GetPackageTripDateBookingDetailDto
                {
                    StartTripDate = bookingTrip.PackageTripDate.StartPackageTripDate,
                    EndTripDate = bookingTrip.PackageTripDate.EndPackageTripDate,
                    PackageTripDateStatus = bookingTrip.PackageTripDate.Status,
                    PackageTripdateId = bookingTrip.PackageTripDateId,
                    ImageUrlPackageTrip = bookingTrip.PackageTripDate.PackageTrip.ImageUrl,
                    PackageTripName = bookingTrip.PackageTripDate.PackageTrip.Name,
                    StartBookingTripDate = bookingTrip.PackageTripDate.StartBookingDate,
                    EndBookingTripDate = bookingTrip.PackageTripDate.EndBookingDate,
                    CanReviewTrip = bookingTrip.PackageTripDate.Status == PackageTripDateStatus.Completed && bookingTrip.BookingStatus == BookingStatus.Completed

                },
                UserId = user.Id,
                ExpireTime = bookingTrip.ExpireTime,
                GetPaymentDto = new GetPaymentDto()
                {
                    PaymentId = bookingTrip.Id,
                    Amount = bookingTrip.Payment.Amount,
                    PaymentDate = bookingTrip.Payment.PaymentDate,
                    PaymentMethodName = bookingTrip.Payment.PaymentMethod.Name,
                    PaymentStatus = bookingTrip.Payment.PaymentStatus,
                    TransactionRef = bookingTrip.Payment.TransactionRef,
                    PaymentInstructions = bookingTrip.Payment.PaymentInstructions ?? string.Empty,
                    CanCompletePayment = string.IsNullOrEmpty(bookingTrip.Payment.TransactionRef) && bookingTrip.Payment.PaymentStatus == PaymentStatus.Pending


                }


            };
            return Result<GetBookingTripForUserDto>.Success(result);
        }
    }
}



