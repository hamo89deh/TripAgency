using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Feature.Payment;


namespace TripAgency.Service.Abstracts
{
    public interface IPaymentService 
    {
        Task<Result> SubmitManualPaymentNotificationAsync(ManualPaymentDetailsDto details);
        Task<Result> ProcessManualPaymentConfirmationAsync(ManualPaymentConfirmationRequestDto request);
        Task<Result> ReportMissingPaymentAsync(MissingPaymentReportDto reportDto);
        Task<Result> ResolveMissingPaymentReportAsync(DiscrepancyReportProcessRequestDto discrepancyReport);
        Task<Result<PaymentTransactionStatusDto>> GetDetailsTransactionAsync(string transactionReference);
        Task<Result<IEnumerable<MissingPaymentReportResponceDto>>> GetMissingPaymentReportsForAdminAsync();
        Task<Result> HandlePaymentTimeoutAsync(int bookingId); // تستدعى من خدمة المؤقت

        Task<Result<IEnumerable<ManualPaymentDetailsDto>>> GetPendingManualPaymentsForAdminAsync();
        Task<Result<IEnumerable<PaymentMethodDto>>> GetPaymentMethods();
    }


}
