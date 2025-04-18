using Microsoft.EntityFrameworkCore;
using ForumSugar.Models.Entities;
using ForumSugar.Repositories.Interfaces;
using ForumSugar.Data;
using System.Linq.Expressions;

namespace ForumSugar.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetCommentByBlogIdAsync(Expression<Func<Comment, bool>> predicate, string[] includes = null)
        {
            IQueryable<Comment> query = _context.Comments;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }
    }
}
