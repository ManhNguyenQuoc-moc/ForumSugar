using ForumSugar.Models.Entities;
using ForumSugar.Repositories;
using ForumSugar.Services.Interfaces;
namespace ForumSugar.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repoNotifi;

        public NotificationService(INotificationRepository repoNotifi)
        {
            _repoNotifi = repoNotifi;
        }

        public async Task SendNotificationAsync(int senderId, int receiverId, string content, NotificationType type)
        {
            var notification = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.Now,
                IsRead = false,
                Type = type
            };

            await _repoNotifi.AddNotificationAsync(notification);
        }
        public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId)
        {
            // Giả sử repository của bạn có phương thức lấy thông báo theo userId
            return await _repoNotifi.GetNotificationsByUserIdAsync(userId);
        }
    }
}
