namespace TripAgency.Service.Abstracts
{
    public interface IPaymentTimerService
    {
        void StartPaymentTimer(int bookingId, TimeSpan timeoutDuration);
        void StopPaymentTimer(int bookingId);
    }
}
