namespace TripAgency.Service.Abstracts
{
    public interface IPaymentGatewayFactory
    {
        IPaymentGatewayService GetGatewayService(string gatewayProviderName);
    }
}
