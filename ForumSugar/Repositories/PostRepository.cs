using ForumSugar.Data;
using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Repositories.Interfaces;
using ForumSugar.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace ForumSugar.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId)
        {
            return await _context.Posts
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> SearchAsync(string keyword)
        {
            return await _context.Posts
                .Where(b => b.Title.Contains(keyword) || b.Content.Contains(keyword))
                .ToListAsync();
        }

        public async Task<List<Post>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Posts
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Posts.CountAsync();
        }
        public async Task<PagedResult<PostDto>> GetPagedAsync(int page, int pageSize, int? currentUserId = null)
        {
            var query = _context.Posts
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new PostDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt,
                    AuthorName = x.User.Name,
                    AvatarUrl = x.User.AvatarUrl,
                    ImageUrl = x.ImageUrl,
                    IsLikedByCurrentUser = currentUserId.HasValue && x.Likes.Contains(currentUserId.Value),
                    LikeCount = x.Likes.Count
                });

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<PostDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }


    }

}
