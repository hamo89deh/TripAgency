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
            //TODO
            // لا يوجد API حقيقي للدفع، فقط نعطي العميل المعلومات ليقوم بالتحويل يدويا
            var instructions = $"يرجى تحويل {request.Amount} {request.Currency} إلى محفظة USDT التالية عبر شبكة {_usdtNetworkType}:\n{_usdtWalletAddress}\nيرجى حفظ رقم عملية التحويل (Transaction Hash) لتقديمه لاحقاً.";
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

    public class PayeerPaymentGatewayService : IPaymentGatewayService
    {
        private readonly ILogger<PayeerPaymentGatewayService> _logger;
        private readonly string _payeerWalletAddress; //TODO
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
            var instructions = $"يرجى تحويل {request.Amount} {request.Currency} إلى محفظة Payeer :\n{_payeerWalletAddress}\nيرجى حفظ رقم عملية التحويل (Transaction Hash) لتقديمه لاحقاً.";
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
    //public class ManualPaymentDetailsDto
    //{
    //    public int BookingId { get; set; }
    //    public string TransactionReference { get; set; } = string.Empty;
    //    public decimal PaidAmount { get; set; }
    //    public string PaymentMethodName { get; set; } = string.Empty;
    //    public DateTime NotificationDate { get; set; } = DateTime.UtcNow;
    //    public string? CustomerNotes { get; set; }
    //}
    //public class ManualPaymentConfirmationRequestDto
    //{
    //    public int BookingId { get; set; }
    //    public bool IsConfirmed { get; set; }
    //    public string AdminNotes { get; set; } = string.Empty;
    //    public string PaymentMethodName { get; set; } = string.Empty;
    //}




    //public class MockPaymentGatewayService : IPaymentGatewayService
    //{
    //    private readonly ILogger<MockPaymentGatewayService> _logger;
    //    private static readonly Dictionary<string, PaymentRequest> _pendingTransactions = new Dictionary<string, PaymentRequest>();

    //    public MockPaymentGatewayService(ILogger<MockPaymentGatewayService> logger)
    //    {
    //        _logger = logger;
    //    }

    //    public Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(PaymentRequest request)
    //    {
    //        _logger.LogInformation("Mock PG: Initiating payment for Booking {BookingId}.", request.BookingId);
    //        var transactionId = Guid.NewGuid().ToString();
    //        _pendingTransactions[transactionId] = request;

    //        var redirectUrl = $"{request.SuccessCallbackUrl.Split('?')[0].Replace("success", "mock-payment-page")}?transactionId={transactionId}&amount={request.Amount}&currency={request.Currency}&successUrl={Uri.EscapeDataString(request.SuccessCallbackUrl)}&failureUrl={Uri.EscapeDataString(request.FailureCallbackUrl)}&cancelUrl={Uri.EscapeDataString(request.CancelCallbackUrl)}";

    //        var response = new PaymentInitiationResponse
    //        {
    //            RedirectUrl = redirectUrl,
    //            TransactionId = transactionId,
    //            IsSuccess = true
    //        };
    //        return Task.FromResult(Result<PaymentInitiationResponse>.Success(response));
    //    }

    //    public Task<Result<PaymentCallbackResult>> ProcessPaymentCallbackAsync(string transactionId, string statusFromGateway, Dictionary<string, string>? additionalParams = null)
    //    {
    //        _logger.LogInformation("Mock PG Callback: Received callback for {TransactionId} with Status {Status}.", transactionId, statusFromGateway);
    //        if (!_pendingTransactions.TryGetValue(transactionId, out var originalRequest))
    //        {
    //            _logger.LogWarning("Mock PG Callback: Transaction {TransactionId} not found or expired.", transactionId);
    //            return Task.FromResult(Result<PaymentCallbackResult>.Failure("Transaction not found or expired in mock gateway.", null, ResultFailureType.NotFound));
    //        }

    //        var callbackResult = new PaymentCallbackResult
    //        {
    //            TransactionId = transactionId,
    //            PaymentMethod = "MockCard",
    //            Amount = originalRequest.Amount
    //        };

    //        switch (statusFromGateway.ToLower())
    //        {
    //            case "success": callbackResult.IsSuccess = true; callbackResult.PaymentStatus = "completed"; break;
    //            case "failed": callbackResult.IsSuccess = false; callbackResult.PaymentStatus = "failed"; callbackResult.ErrorDetails = "Payment rejected by mock gateway."; break;
    //            case "cancelled": callbackResult.IsSuccess = false; callbackResult.PaymentStatus = "cancelled"; callbackResult.ErrorDetails = "Payment cancelled by mock gateway."; break;
    //            default: callbackResult.IsSuccess = false; callbackResult.PaymentStatus = "unknown"; callbackResult.ErrorDetails = "Unknown status from mock gateway."; break;
    //        }

    //        _pendingTransactions.Remove(transactionId);
    //        return Task.FromResult(Result<PaymentCallbackResult>.Success(callbackResult));
    //    }
    //}

    //public class StripePaymentGatewayService : IPaymentGatewayService
    //{
    //    private readonly IConfiguration _configuration;
    //    private readonly ILogger<StripePaymentGatewayService> _logger;
    //    private readonly string _stripeSecretKey;

    //    public StripePaymentGatewayService(IConfiguration configuration, ILogger<StripePaymentGatewayService> logger)
    //    {
    //        _configuration = configuration;
    //        _logger = logger;
    //        _stripeSecretKey = _configuration["PaymentGateways:Stripe:SecretKey"] ?? throw new InvalidOperationException("Stripe SecretKey not configured.");
    //        // StripeConfiguration.ApiKey = _stripeSecretKey; // تهيئة Stripe SDK
    //    }

    //    public async Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(PaymentRequest request)
    //    {
    //        try
    //        {
    //            _logger.LogInformation("Stripe: Initiating payment for Booking {BookingId}.", request.BookingId);
    //            // هذا هو المكان اللي بتستخدم فيه Stripe.net SDK أو HttpClient لإنشاء Checkout Session

    //            var options = new SessionCreateOptions
    //            {
    //                PaymentMethodTypes = new List<string> { "card" },
    //                LineItems = new List<SessionLineItemOptions> {
    //                    new SessionLineItemOptions {
    //                        PriceData = new SessionLineItemPriceDataOptions { Currency = request.Currency.ToLower(), UnitAmount = (long)(request.Amount * 100), ProductData = new SessionLineItemPriceDataProductDataOptions { Name = $"Booking {request.BookingId}" } },
    //                        Quantity = 1,
    //                    },
    //                },
    //                Mode = "payment",
    //                SuccessUrl = request.SuccessCallbackUrl,
    //                CancelUrl = request.CancelCallbackUrl,
    //                ClientReferenceId = request.BookingId.ToString(),
    //                CustomerEmail = request.CustomerEmail
    //            };
    //            var service = new SessionService();
    //            Session session = await service.CreateAsync(options);

    //            var response = new PaymentInitiationResponse { RedirectUrl = session.Url, TransactionId = session.Id, IsSuccess = true };
    //            return Result<PaymentInitiationResponse>.Success(response);

    //            //    // كود وهمي لـ Stripe
    //            //    var dummyStripeUrl = $"https://stripe.com/mock-checkout?session_id=cs_{Guid.NewGuid().ToString().Replace("-", "")}&amount={request.Amount}&status=mock_pending";
    //            //    return Result<PaymentInitiationResponse>.Success(new PaymentInitiationResponse { RedirectUrl = dummyStripeUrl, TransactionId = $"cs_{Guid.NewGuid()}", IsSuccess = true });
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Stripe: Error initiating payment for Booking {BookingId}.", request.BookingId);
    //            return Result<PaymentInitiationResponse>.BadRequest($"Error initiating Stripe payment: {ex.Message}");
    //        }
    //    }

    //    public async Task<Result<PaymentCallbackResult>> ProcessPaymentCallbackAsync(string transactionId, string statusFromGateway, Dictionary<string, string>? additionalParams = null)
    //    {
    //        try
    //        {
    //            _logger.LogInformation("Stripe Callback: Processing callback for {TransactionId} with Status {Status}.", transactionId, statusFromGateway);
    //            // هنا يتم التحقق من توقيع الـ webhook ومعلومات الـ session من Stripe API
    //            /*
    //            var service = new SessionService();
    //            Session session = await service.GetAsync(transactionId); // transactionId هنا هو Session ID
    //            var callbackResult = new PaymentCallbackResult {
    //                TransactionId = session.Id, PaymentMethod = session.PaymentMethodTypes?.FirstOrDefault() ?? "Unknown",
    //                Amount = session.AmountTotal.HasValue ? (decimal)session.AmountTotal.Value / 100M : 0M,
    //                IsSuccess = session.PaymentStatus == "paid", PaymentStatus = session.PaymentStatus,
    //            };
    //            */
    //            // كود وهمي لـ Stripe Callback
    //            var isSuccess = statusFromGateway.Equals("success", StringComparison.OrdinalIgnoreCase) || statusFromGateway.Equals("paid", StringComparison.OrdinalIgnoreCase);
    //            var callbackResult = new PaymentCallbackResult
    //            {
    //                TransactionId = transactionId,
    //                PaymentMethod = "Stripe Card",
    //                IsSuccess = isSuccess,
    //                PaymentStatus = isSuccess ? "completed" : statusFromGateway.ToLower(),
    //                ErrorDetails = isSuccess ? "" : "Stripe mock payment failed."
    //            };
    //            return Result<PaymentCallbackResult>.Success(callbackResult);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Stripe Callback: Error processing callback for {TransactionId}.", transactionId);
    //            return Result<PaymentCallbackResult>.BadRequest($"Error processing Stripe callback: {ex.Message}");
    //        }
    //    }
    //}

    //public class SyriatelCashPaymentGatewayService : IPaymentGatewayService
    //{
    //    private readonly HttpClient _httpClient;
    //    private readonly IConfiguration _configuration;
    //    private readonly ILogger<SyriatelCashPaymentGatewayService> _logger;

    //    private readonly string _merchantId;
    //    private readonly string _merchantPassword;
    //    private readonly string _syriatelApiUrl;
    //    private readonly string _syriatelPhoneNumber; // رقم الهاتف الذي سيدفع عليه العميل

    //    public SyriatelCashPaymentGatewayService(
    //        HttpClient httpClient,
    //        IConfiguration configuration,
    //        ILogger<SyriatelCashPaymentGatewayService> logger)
    //    {
    //        _httpClient = httpClient;
    //        _configuration = configuration;
    //        _logger = logger;

    //        _merchantId = _configuration["PaymentGateways:SyriatelCash_Manual:MerchantId"] ?? throw new InvalidOperationException("SyriatelCash MerchantId not configured.");
    //        _merchantPassword = _configuration["PaymentGateways:SyriatelCash_Manual:Password"] ?? throw new InvalidOperationException("SyriatelCash Password not configured.");
    //        _syriatelApiUrl = _configuration["PaymentGateways:SyriatelCash_Manual:ApiUrl"] ?? throw new InvalidOperationException("SyriatelCash ApiUrl not configured.");
    //        _syriatelPhoneNumber = _configuration["PaymentGateways:SyriatelCash_Manual:PhoneNumber"] ?? throw new InvalidOperationException("SyriatelCash PhoneNumber not configured.");

    //        _httpClient.BaseAddress = new Uri(_syriatelApiUrl);
    //        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    //    }

    //    public Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(PaymentRequest request)
    //    {
    //        _logger.LogInformation("Syriatel Cash Manual Payment: تهيئة دفع يدوي للحجز {BookingId} بمبلغ {Amount}.", request.BookingId, request.Amount);

    //        // لا يوجد API حقيقي للدفع، فقط نعطي العميل المعلومات ليقوم بالتحويل يدويا
    //        var instructions = $"يرجى تحويل مبلغ {request.Amount} {request.Currency} إلى الرقم التالي عبر خدمة Syriatel Cash:\n{_syriatelPhoneNumber}\nيرجى حفظ رقم عملية الدفع (Transaction ID) الذي يصلك عبر رسالة نصية.";

    //        var response = new PaymentInitiationResponse
    //        {
    //            RedirectUrl = $"{request.SuccessCallbackUrl.Split('?')[0].Replace("success", "manual-payment-info")}" + // URL لصفحة معلومات الدفع اليدوي
    //                          $"?bookingId={request.BookingId}&amount={request.Amount}&currency={request.Currency}" +
    //                          $"&phone={Uri.EscapeDataString(_syriatelPhoneNumber)}&instructions={Uri.EscapeDataString(instructions)}",
    //            TransactionId = request.BookingId.ToString(), // نستخدم BookingId كمعرف مبدئي
    //            IsSuccess = true, // التهيئة ناجحة لأننا أعطينا العميل المعلومات
    //            ErrorMessage = instructions // رسالة التعليمات تظهر كخطأ مبدئي
    //        };
    //        return Task.FromResult(Result<PaymentInitiationResponse>.Success(response));
    //    }

    //    public Task<Result<PaymentCallbackResult>> ProcessPaymentCallbackAsync(string transactionId, string statusFromGateway, Dictionary<string, string>? additionalParams = null)
    //    {
    //        // هذا الجزء لن يتم استدعاؤه تلقائياً من بوابة الدفع لـ Syriatel Cash (الدفع يدوي).
    //        // معالجة تأكيد الدفع لـ Syriatel Cash ستتم يدوياً من قبل Admin.
    //        _logger.LogWarning("Syriatel Cash Manual Payment: Unexpected callback for {TransactionId} with status {Status}. This service does not process automatic callbacks.", transactionId, statusFromGateway);
    //        return Task.FromResult(Result<PaymentCallbackResult>.Failure("هذه الخدمة لا تدعم ردود النداء التلقائية للدفع اليدوي.", null, ResultFailureType.Forbidden));
    //    }
    //}



    //public class SyriatelCashPaymentGatewayService : IPaymentGatewayService
    //{
    //    private readonly HttpClient _httpClient;
    //    private readonly IConfiguration _configuration;
    //    private readonly ILogger<SyriatelCashPaymentGatewayService> _logger;

    //    private readonly string _merchantId;
    //    private readonly string _merchantPassword;
    //    private readonly string _syriatelApiUrl;
    //    private readonly string _syriatelPhoneNumber; // رقم الهاتف الذي سيدفع عليه العميل

    //    public SyriatelCashPaymentGatewayService(
    //        HttpClient httpClient,
    //        IConfiguration configuration,
    //        ILogger<SyriatelCashPaymentGatewayService> logger)
    //    {
    //        _httpClient = httpClient;
    //        _configuration = configuration;
    //        _logger = logger;

    //        _merchantId = _configuration["PaymentGateways:SyriatelCash_Manual:MerchantId"] ?? throw new InvalidOperationException("SyriatelCash MerchantId not configured.");
    //        _merchantPassword = _configuration["PaymentGateways:SyriatelCash_Manual:Password"] ?? throw new InvalidOperationException("SyriatelCash Password not configured.");
    //        _syriatelApiUrl = _configuration["PaymentGateways:SyriatelCash_Manual:ApiUrl"] ?? throw new InvalidOperationException("SyriatelCash ApiUrl not configured.");
    //        _syriatelPhoneNumber = _configuration["PaymentGateways:SyriatelCash_Manual:PhoneNumber"] ?? throw new InvalidOperationException("SyriatelCash PhoneNumber not configured.");

    //        _httpClient.BaseAddress = new Uri(_syriatelApiUrl);
    //        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    //    }

    //    public Task<Result<PaymentInitiationResponse>> InitiatePaymentAsync(PaymentRequest request)
    //    {
    //        _logger.LogInformation("Syriatel Cash Manual Payment: تهيئة دفع يدوي للحجز {BookingId} بمبلغ {Amount}.", request.BookingId, request.Amount);

    //        // لا يوجد API حقيقي للدفع، فقط نعطي العميل المعلومات ليقوم بالتحويل يدويا
    //        var instructions = $"يرجى تحويل مبلغ {request.Amount} {request.Currency} إلى الرقم التالي عبر خدمة Syriatel Cash:\n{_syriatelPhoneNumber}\nيرجى حفظ رقم عملية الدفع (Transaction ID) الذي يصلك عبر رسالة نصية.";

    //        var response = new PaymentInitiationResponse
    //        {
    //            RedirectUrl = $"{request.SuccessCallbackUrl.Split('?')[0].Replace("success", "manual-payment-info")}" + // URL لصفحة معلومات الدفع اليدوي
    //                          $"?bookingId={request.BookingId}&amount={request.Amount}&currency={request.Currency}" +
    //                          $"&phone={Uri.EscapeDataString(_syriatelPhoneNumber)}&instructions={Uri.EscapeDataString(instructions)}",
    //            TransactionId = request.BookingId.ToString(), // نستخدم BookingId كمعرف مبدئي
    //            IsSuccess = true, // التهيئة ناجحة لأننا أعطينا العميل المعلومات
    //            ErrorMessage = instructions // رسالة التعليمات تظهر كخطأ مبدئي
    //        };
    //        return Task.FromResult(Result<PaymentInitiationResponse>.Success(response));
    //    }

    //    public Task<Result<PaymentCallbackResult>> ProcessPaymentCallbackAsync(string transactionId, string statusFromGateway, Dictionary<string, string>? additionalParams = null)
    //    {
    //        // هذا الجزء لن يتم استدعاؤه تلقائياً من بوابة الدفع لـ Syriatel Cash (الدفع يدوي).
    //        // معالجة تأكيد الدفع لـ Syriatel Cash ستتم يدوياً من قبل Admin.
    //        _logger.LogWarning("Syriatel Cash Manual Payment: Unexpected callback for {TransactionId} with status {Status}. This service does not process automatic callbacks.", transactionId, statusFromGateway);
    //        return Task.FromResult(Result<PaymentCallbackResult>.Failure("هذه الخدمة لا تدعم ردود النداء التلقائية للدفع اليدوي.", null, ResultFailureType.Forbidden));
    //    }
    //}




}
