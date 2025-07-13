namespace TripAgency.Service.Abstracts
{
    public interface INotificationService 
    {
        public  Task NotifyTripCancellation(int packageTripDateId);
        public Task NotifyTripCompletion(int packageTripDateId);
    }
}
