using TripAgency.Data.Entities;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.BookingTrip.Queries;
using TripAgency.Service.Feature.BookingTrip.Commands;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Payment;
using TripAgency.Service.Implementations;


namespace TripAgency.Service.Abstracts
{
    public interface IBookingTripService : IGenericService<BookingTrip, GetBookingTripByIdDto, GetBookingTripsDto , AddBookingTripDto, UpdateBookingTripDto>                                  
    {
        Task<Result<PaymentInitiationResponseDto>> InitiateBookingAndPaymentAsync(AddBookingPackageTripDto bookPackageDto);
      

    }
    public interface IPaymentService 
    {
        Task<Result> SubmitManualPaymentNotificationAsync(ManualPaymentDetailsDto details);
        Task<Result> ProcessManualPaymentConfirmationAsync(ManualPaymentConfirmationRequestDto request);
        Task<Result> ReportMissingPaymentAsync(MissingPaymentReportDto reportDto, int userId);
        Task<Result> ResolveMissingPaymentReportAsync(DiscrepancyReportProcessRequestDto discrepancyReport);
        Task<Result<PaymentTransactionStatusDto>> GetDetailsTransactionAsync(string transactionReference);
        Task<Result<IEnumerable<MissingPaymentReportResponceDto>>> GetMissingPaymentReportsForAdminAsync();
        Task<Result> HandlePaymentTimeoutAsync(int bookingId); // تستدعى من خدمة المؤقت

        //Task<Result> ProcessPaymentCallbackAsync(PaymentCallbackRequestDto callbackRequest);



        //Task<Result<IEnumerable<ManualPaymentDetailsDto>>> GetPendingManualPaymentsForAdminAsync();

        //// وظائف جلب تفاصيل الحجوزات
        //Task<Result<BookingDto>> GetBookingDetailsAsync(int bookingId);
        //Task<Result<IEnumerable<BookingDto>>> GetAllBookingsAsync(int? userId = null);


        //Task<Result> ResolveMissingPaymentReportAsync(int reportId, bool isConfirmed, string adminNotes, int adminUserId);

        // 11. جلب بلاغات الدفع المفقودة للمدير (البلاغات بانتظار المراجعة)
        // Task<Result<IEnumerable<MissingPaymentReportDto>>> GetMissingPaymentReportsForAdminAsync();

        // 12. معالجة بلاغ دفعة مفقودة من قبل المدير (تأكيد/رفض)
    }


}
