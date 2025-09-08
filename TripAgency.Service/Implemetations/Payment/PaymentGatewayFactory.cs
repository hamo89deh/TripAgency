using Microsoft.Extensions.DependencyInjection;
using TripAgency.Service.Abstracts;

namespace TripAgency.Service.Implemetations.Payment
{
    public class PaymentGatewayFactory : IPaymentGatewayFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentGatewayFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentGatewayService GetGatewayService(string gatewayProviderName)
        {
            return gatewayProviderName!.ToLower() switch
            {
                // "stripe" => _serviceProvider.GetRequiredService<StripePaymentGatewayService>(),
                "usdt" => _serviceProvider.GetRequiredService<UsdtPaymentGatewayService>(),
                "payeer" => _serviceProvider.GetRequiredService<PayeerPaymentGatewayService>(),
                "syriatel cash" => _serviceProvider.GetRequiredService<SyriatelCashPaymentGatewayService>(),
                // أضف أي بوابات دفع أخرى هنا (مثل PayPal, Payeer)
                //"mock" => _serviceProvider.GetRequiredService<MockPaymentGatewayService>(), // خيار الـ Mock للمحاكاة
                _ => throw new ArgumentException($"Payment gateway '{gatewayProviderName}' not supported or not configured in factory.")
            };
        }
    }
}
