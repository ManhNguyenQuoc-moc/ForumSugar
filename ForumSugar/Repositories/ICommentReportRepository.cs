
using ForumSugar.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForumSugar.Repositories.Interfaces
{
    public interface ICommentReportRepository : IGenericRepository<CommentReport>
    {
        Task<IEnumerable<CommentReport>> GetReportedCommentsWithDetailsAsync();
    }
}
