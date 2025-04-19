using ForumSugar.Data;
using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Repositories.Interfaces;
using ForumSugar.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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
        public async Task<PagedResult<PostDto>> GetPagedAsync(int page, int pageSize, int? currentUserId)
        {
            // Lấy dữ liệu từ DB, bao gồm cả User, Topic, Comments
            var query = _context.Posts
                .Where(p => p.isAproved && !p.isLocked)
                .Include(p => p.User)
                .Include(p => p.Topic)
                .Include(p => p.Comments)
                .OrderByDescending(x => x.CreatedAt);

            // Tổng số bản ghi (phân trang)
            var total = await query.CountAsync();

            // Lấy danh sách bài viết phân trang
            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Chuyển sang PostDto và xử lý Likes trong bộ nhớ
            var items = posts.Select(x => new PostDto
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                AuthorName = x.User?.Name ?? "Ẩn danh",
                AvatarUrl = x.User?.AvatarUrl,
                ImageUrl = x.ImageUrl,
                TopicName = x.Topic?.Name ?? "",
                IsLikedByCurrentUser = currentUserId.HasValue && x.Likes != null && x.Likes.Contains(currentUserId.Value),
                LikeCount = x.Likes?.Count ?? 0,
                CommentCount = x.Comments?.Count ?? 0
            }).ToList();

            return new PagedResult<PostDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }
        public async Task<PagedResult<PostDto>> GetPagedBlogsAdminNotApprovedAsync(int page, int pageSize, int? currentUserId = null)
        {
            // Lấy dữ liệu từ DB, bao gồm cả User, Topic, Comments
            var query = _context.Posts
                .Where(p => !p.isAproved && !p.isLocked)
                .Include(p => p.User)
                .Include(p => p.Topic)
                .Include(p => p.Comments)
                .OrderByDescending(x => x.CreatedAt);

            // Tổng số bản ghi (phân trang)
            var total = await query.CountAsync();

            // Lấy danh sách bài viết phân trang
            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Chuyển sang PostDto và xử lý Likes trong bộ nhớ
            var items = posts.Select(x => new PostDto
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                AuthorName = x.User?.Name ?? "Ẩn danh",
                AvatarUrl = x.User?.AvatarUrl,
                ImageUrl = x.ImageUrl,
                TopicName = x.Topic?.Name ?? "",
                IsLikedByCurrentUser = currentUserId.HasValue && x.Likes != null && x.Likes.Contains(currentUserId.Value),
                LikeCount = x.Likes?.Count ?? 0,
                CommentCount = x.Comments?.Count ?? 0
            }).ToList();

            return new PagedResult<PostDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }
        public async Task<PagedResult<PostDto>> GetPagedBlogsUseridAsync(int page, int pageSize, int? currentUserId = null)
        {
            // Lấy dữ liệu từ DB, bao gồm cả User, Topic, Comments
            Console.WriteLine("Current UserId: " + currentUserId);
            var query = _context.Posts
                .Where(p => p.isAproved && !p.isLocked && p.UserId == currentUserId)
                .Include(p => p.User)
                .Include(p => p.Topic)
                .Include(p => p.Comments)
                .OrderByDescending(x => x.CreatedAt);

            // Tổng số bản ghi (phân trang)
            var total = await query.CountAsync();

            // Lấy danh sách bài viết phân trang
            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Chuyển sang PostDto và xử lý Likes trong bộ nhớ
            var items = posts.Select(x => new PostDto
            {
                Id = x.Id,
                Title = x.Title,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                AuthorName = x.User?.Name ?? "Ẩn danh",
                AvatarUrl = x.User?.AvatarUrl,
                ImageUrl = x.ImageUrl,
                TopicName = x.Topic?.Name ?? "",
                IsLikedByCurrentUser = currentUserId.HasValue && x.Likes != null && x.Likes.Contains(currentUserId.Value),
                LikeCount = x.Likes?.Count ?? 0,
                CommentCount = x.Comments?.Count ?? 0
            }).ToList();

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
