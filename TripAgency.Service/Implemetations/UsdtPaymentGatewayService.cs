using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Payment;

namespace TripAgency.Service.Implementations
{
    public class UsdtPaymentGatewayService : IPaymentGatewayService
    {
        private readonly ILogger<UsdtPaymentGatewayService> _logger;
        private readonly string _usdtWalletAddress; 
        private readonly string _usdtNetworkType;
        private readonly string baseUrl;

        public UsdtPaymentGatewayService(IConfiguration configuration, ILogger<UsdtPaymentGatewayService> logger)
        {
            _logger = logger;
            _usdtWalletAddress = configuration["PaymentGateways:USDT_Manual:WalletAddress"] ?? throw new InvalidOperationException("USDT WalletAddress not configured.");
            _usdtNetworkType = configuration["PaymentGateways:USDT_Manual:NetworkType"] ?? throw new InvalidOperationException("USDT NetworkType not configured.");
            baseUrl = configuration["BaseUrl"] ?? throw new InvalidOperationException("USDT WalletAddress not configured.");

        }

        public async Task<Result<PaymentInitiationResponseDto>> InitiatePaymentAsync(PaymentRequest request)
        {
            _logger.LogInformation("USDT Manual Payment: Initiating manual payment for Booking {BookingId}.", request.BookingId);
            var instructions = $"Please tranfer {request.Amount} {request.Currency} to a Wallet Usdt : {_usdtWalletAddress}  Through a NetworkType  {_usdtNetworkType} Save The Transfer Process (Transaction Hash) To send It later";
            var response = new PaymentInitiationResponseDto
            {
                // لا يوجد RedirectUrl حقيقي
                // ، لكن نستخدمه لتمرير معلومات للواجهة الأمامية
                RedirectUrl = $"{baseUrl}/payment/manual-info?bookingId={request.BookingId}&method=usdt",
                TransactionId = string.Empty, 
                IsSuccess = true, // التهيئة ناجحة لأننا أعطينا العميل المعلومات
                PaymentInstructions = instructions
            };
            return Result<PaymentInitiationResponseDto>.Success(response);
        }

        public  Task<Result<PaymentCallbackResult>> ProcessPaymentCallbackAsync(string transactionId, string statusFromGateway, Dictionary<string, string>? additionalParams = null)
        {
            // هذا الجزء لن يتم استدعاؤه تلقائياً من بوابة الدفع لـ USDT.
            // معالجة تأكيد الدفع لـ USDT ستتم يدوياً من قبل Admin (عبر SubmitManualPaymentNotificationAsync).
            _logger.LogWarning("USDT Manual Payment: Unexpected callback for {TransactionId} with status {Status}. This service does not process automatic callbacks.", transactionId, statusFromGateway);
            return Task.FromResult(Result<PaymentCallbackResult>.Failure("هذه الخدمة لا تدعم ردود النداء التلقائية للدفع اليدوي.", null, ResultFailureType.Forbidden));
        }
    }
}
