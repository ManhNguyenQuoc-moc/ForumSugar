using ForumSugar.Models.Entities;

namespace ForumSugar.Repositories
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId);
    }
}
