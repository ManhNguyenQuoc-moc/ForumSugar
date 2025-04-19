using ForumSugar.Data;
using ForumSugar.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForumSugar.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;
        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _context.Notifications
                                 .Where(n => n.ReceiverId == userId)
                                 .ToListAsync();
        }
    }
}
