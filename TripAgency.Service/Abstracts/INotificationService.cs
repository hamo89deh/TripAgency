using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;

namespace TripAgency.Service.Abstracts
{
    public interface INotificationService 
    {
        Task<Result> CreateInAppNotificationAsync(int userId, string title, string message, string type, string? relatedEntityId = null);
        Task<Result<IEnumerable<Notification>>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task<Result> MarkNotificationAsReadAsync(int notificationId, int userId);

    }
}
