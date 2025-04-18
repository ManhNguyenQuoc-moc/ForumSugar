using ForumSugar.Repositories;
using System.Linq.Expressions;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentByBlogIdAsync(Expression<Func<Comment, bool>> predicate, string[] includes = null);
}
