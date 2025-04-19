using ForumSugar.Models.Entities;

namespace ForumSugar.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId);
        Task SendNotificationAsync(int senderId, int receiverId, string content, NotificationType type);
    }

}
