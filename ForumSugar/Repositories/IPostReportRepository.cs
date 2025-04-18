using ForumSugar.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForumSugar.Repositories.Interfaces
{
    public interface IPostReportRepository : IGenericRepository<PostReport> 
    {
        Task<IEnumerable<PostReport>> GetReportsByBlogIdAsync(int blogId);
        Task<bool> HasUserReportedAsync(int blogId, int userId);
    }
}
