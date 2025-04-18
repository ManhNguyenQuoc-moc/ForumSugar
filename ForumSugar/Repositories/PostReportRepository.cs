using ForumSugar.Data;
using ForumSugar.Models.Entities;
using ForumSugar.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSugar.Repositories
{
    public class PostReportRepository : GenericRepository<PostReport>, IPostReportRepository
    {
        private readonly AppDbContext _context;

        public PostReportRepository(AppDbContext context) :base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostReport>> GetReportsByBlogIdAsync(int postId)
        {
            return await _context.PostReports
                .Where(r => r.PostId == postId)
                .ToListAsync();
        }

        public async Task<bool> HasUserReportedAsync(int postId, int userId)
        {
            return await _context.PostReports
                .AnyAsync(r => r.PostId == postId && r.UseId == userId);
        }

        // Optional: Bạn có thể thêm các phương thức bổ sung nếu cần
        public async Task<bool> AddAsync(PostReport report)
        {
            await _context.PostReports.AddAsync(report);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.PostReports.FindAsync(id);
            if (report == null) return false;

            _context.PostReports.Remove(report);
            return await _context.SaveChangesAsync() > 0;
        }
        
    }
}
