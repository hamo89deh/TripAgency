using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Payment;


namespace TripAgency.Service.Abstracts
{
    public interface IPaymentGatewayService
    {
        Task<Result<PaymentInitiationResponseDto>> InitiatePaymentAsync(PaymentRequest request);
       // Task<Result<PaymentCallbackResult>> ProcessPaymentCallbackAsync(string transactionId, string statusFromGateway, Dictionary<string, string>? additionalParams = null);
    }
}
