using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Payment;

namespace TripAgency.Service.Implementations
{
    public class PayeerPaymentGatewayService : IPaymentGatewayService
    {
        private readonly ILogger<PayeerPaymentGatewayService> _logger;
        private readonly string _payeerWalletAddress; 
        private readonly string baseUrl;

        public PayeerPaymentGatewayService(IConfiguration configuration, ILogger<PayeerPaymentGatewayService> logger)
        {
            _logger = logger;
            _payeerWalletAddress = configuration["PaymentGateways:Payeer:WalletAddress"] ?? throw new InvalidOperationException("USDT WalletAddress not configured.");
            baseUrl = configuration["BaseUrl"] ?? throw new InvalidOperationException("Not Found BaseUrl.");
        }

        public async Task<Result<PaymentInitiationResponseDto>> InitiatePaymentAsync(PaymentRequest request)
        {
            _logger.LogInformation($"Payeer Payment: Initiating manual payment for Booking {request.BookingId}.");
            //TODO
            // لا يوجد API حقيقي للدفع، فقط نعطي العميل المعلومات ليقوم بالتحويل يدويا
            var instructions = $"Please tranfer {request.Amount} {request.Currency} to a Wallet Payeer : {_payeerWalletAddress} Save The Transfer Process (Transaction Hash) To send It later";
            var response = new PaymentInitiationResponseDto
            {
                // لا يوجد RedirectUrl حقيقي
                // ، لكن نستخدمه لتمرير معلومات للواجهة الأمامية
                RedirectUrl = $"{baseUrl}/payment/manual-info?bookingId={request.BookingId}&method=Payeer",
                TransactionId = string.Empty,
                IsSuccess = true, // التهيئة ناجحة لأننا أعطينا العميل المعلومات
                PaymentInstructions = instructions
            };
            return Result<PaymentInitiationResponseDto>.Success(response);
        }

        public Task<Result<PaymentCallbackResult>> ProcessPaymentCallbackAsync(string transactionId, string statusFromGateway, Dictionary<string, string>? additionalParams = null)
        {
            // هذا الجزء لن يتم استدعاؤه تلقائياً من بوابة الدفع لـ USDT.
            // معالجة تأكيد الدفع لـ USDT ستتم يدوياً من قبل Admin (عبر SubmitManualPaymentNotificationAsync).
            _logger.LogWarning("Payeer Manual Payment: Unexpected callback for {TransactionId} with status {Status}. This service does not process automatic callbacks.", transactionId, statusFromGateway);
            return Task.FromResult(Result<PaymentCallbackResult>.Failure("هذه الخدمة لا تدعم ردود النداء التلقائية للدفع اليدوي.", null, ResultFailureType.Forbidden));
        }
    }
}
