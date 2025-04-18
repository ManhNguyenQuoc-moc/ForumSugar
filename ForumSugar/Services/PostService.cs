using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Models.Responses;
using ForumSugar.Repositories.Interfaces;
using ForumSugar.Services.Interfaces;
using ForumSugar.Wrappers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ForumSugar.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepo;
        private readonly IPostReportRepository _reportRepo;

        public PostService(IPostRepository PostRepo, IPostReportRepository reportRepo)
        {
            _postRepo = PostRepo;
            _reportRepo = reportRepo;
        }

        public async Task<ApiResponse<IEnumerable<Post>>> GetAllAsync()
        {
            var blogs = await _postRepo.GetAllAsync();
            return new ApiResponse<IEnumerable<Post>>(true, "Lấy danh sách blog thành công", blogs);
        }

        public async Task<ApiResponse<Post>> GetByIdAsync(int id)
        {
            var post = await _postRepo.GetByIdAsync(id);
            if (post == null)
                return new ApiResponse<Post>(false, "Blog not found", null);  // Khi không tìm thấy blog
            return new ApiResponse<Post>(true, "Lấy danh sách blog thành công", post);  // Khi tìm thấy blog, trả về dữ liệu
        }

        public async Task<ApiResponse<Post>> CreateAsync(CreatePostDto dto)
        {
         try { 
            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                UserId = dto.UserId,
                TopicId = dto.TopicId,
                isAproved = false,
                isLocked = false,
                CreatedAt = DateTime.Now
            };

            await _postRepo.AddAsync(post);
            return new ApiResponse<Post>(true, "Tạo Blog thành công", post);
        }
    
            catch (Exception ex)
            {
                return new ApiResponse<Post>(false, "Đã xảy ra lỗi: " + ex.Message, null);
            }
        }

        public async Task<ApiResponse<Post>> UpdateAsync(int id, Post post)
        {
            var existing = await _postRepo.GetByIdAsync(id);
            if (existing == null)
                return new ApiResponse<Post>(false, "Blog not found", null);

            existing.Title = post.Title;
            existing.Content = post.Content;
            existing.UpdatedAt = DateTime.Now;

            await _postRepo.UpdateAsync(existing);
            return new ApiResponse<Post>(true, "Cập nhật blog thành công", existing);
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var post = await _postRepo.GetByIdAsync(id);
            if (post == null)
                return new ApiResponse<string>(false, "post not found", null);

            await _postRepo.DeleteAsync(post);
            return new ApiResponse<string>(true ,"post deleted",null);
        }

        public async Task<ApiResponse<IEnumerable<Post>>> GetPostsByUserIdAsync(int userId)
        {
            var posts = await _postRepo.GetAllAsync();
            var userposts = posts.Where(b => b.UserId == userId);
            return new ApiResponse<IEnumerable<Post>>(true, "Lấy danh sách post củangười dùng "+userId +"thành công", userposts);
        }

        public async Task<ApiResponse<IEnumerable<Post>>> SearchBlogsAsync(string keyword)
        {
            var posts = await _postRepo.GetAllAsync();
            var result = posts.Where(b =>
                (!string.IsNullOrEmpty(b.Title) && b.Title.Contains(keyword)) ||
                (!string.IsNullOrEmpty(b.Content) && b.Content.Contains(keyword))
            );

            return new ApiResponse<IEnumerable<Post>>(true, "Lấy danh sách post thành công", result);
        }

        public async Task<ApiResponse<string>> ReportPostAsync(int postId, string reason)
        {
            var blog = await _postRepo.GetByIdAsync(postId);
            if (blog == null)
                return new ApiResponse<string>(false, "Blog not found", null);

            await _reportRepo.AddAsync(new PostReport
            {
                PostId = postId,
                Reason = reason,
                ReportedAt = DateTime.Now
            });

            return new ApiResponse<string>(true, "Reported successfully", null);
        }

        //public async Task<ApiResponse<PagedResult<Post>>> GetPagedPostsAsync(int pageNumber, int pageSize, string? topic = null)
        //{
        //    var query = _postRepo.GetAllAsQueryable()
        //         .Include(p => p.Topic) // Tải thông tin Topic liên kết
        //         .OrderByDescending(p => p.CreatedAt);

        //    if (!string.IsNullOrEmpty(topic))
        //    {
        //        query = query.Where(p => p.Topic.Name == topic).OrderByDescending(p => p.CreatedAt);
        //    }
        //    var totalRecords = await query.CountAsync();
        //    var items = await query
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var pagedResult = new PagedResult<Post>(
        //           data: items,  // Đảm bảo rằng bạn sử dụng đúng thuộc tính Data
        //           totalCount: totalRecords,
        //           pageIndex: pageNumber,
        //           pageSize: pageSize
        //       );

        //    return new ApiResponse<PagedResult<Post>>( true, "Lấy danh sách bài viết thành công", pagedResult);
        //}

        public async Task<ApiResponse<string>> ApprovePostAsync(int postId)
        {
            var post = await _postRepo.GetByIdAsync(postId);
            if (post == null)
                return new ApiResponse<string>(false,"Blog not found");

            post.isAproved = true;
            await _postRepo.UpdateAsync(post);
            return new ApiResponse<string>(true,"Blog approved");
        }
        public async Task<ApiResponse<string>> LikePostAsync(int blogId, int userId)
        {
            var blog = await _postRepo.GetByIdAsync(blogId);
            if (blog == null)
                return new ApiResponse<string>(false, "Blog not found");

            // Kiểm tra xem blog có danh sách likes chưa, nếu chưa thì khởi tạo
            blog.Likes ??= new List<int>();

            // Nếu người dùng đã like thì thực hiện unlike (xóa like)
            if (blog.Likes.Contains(userId))
            {
                blog.Likes.Remove(userId); // Xóa like của user
                await _postRepo.UpdateAsync(blog);
                return new ApiResponse<string>(true, "Unliked blog"); // Trả về thông báo bỏ like
            }
            else
            {
                // Nếu người dùng chưa like thì thực hiện like (thêm like)
                blog.Likes.Add(userId); // Thêm like của user
                await _postRepo.UpdateAsync(blog);
                return new ApiResponse<string>(true, "Liked blog"); // Trả về thông báo đã like
            }
        }
        public async Task<ApiResponse<IEnumerable<Post>>> SearchPostsAsync(string keyword)
        {
            var posts = await _postRepo.GetAllAsync();
            var result = posts.Where(b =>
                (!string.IsNullOrEmpty(b.Title) && b.Title.Contains(keyword)) ||
                (!string.IsNullOrEmpty(b.Content) && b.Content.Contains(keyword))
            );

            return new ApiResponse<IEnumerable<Post>>(true, "Lấy danh sách post thành công", result);
        }
        public async Task<PagedResult<PostDto>> GetPagedBlogsAsync(int page, int pageSize, int? currentUserId)
        {
            return await _postRepo.GetPagedAsync(page, pageSize, currentUserId);
        }


    }
}
