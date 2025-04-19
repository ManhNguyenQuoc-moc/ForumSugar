using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Models.Responses;
using ForumSugar.Repositories;
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
        private readonly IPhotoService _photoService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        public PostService(IPostRepository PostRepo, IPostReportRepository reportRepo, IPhotoService photoService, INotificationService notificationService)
        {
            _postRepo = PostRepo;
            _reportRepo = reportRepo;
            _photoService = photoService;
            _notificationService = notificationService;
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
            if (dto == null)
            {
                return new ApiResponse<Post>(false, "Dữ liệu bài viết không hợp lệ.", null);
            }

            // Kiểm tra các thuộc tính cơ bản
            if (string.IsNullOrEmpty(dto.Title) || string.IsNullOrEmpty(dto.Content))
            {
                return new ApiResponse<Post>(false, "Tiêu đề và nội dung không được để trống.", null);
            }

            if (dto.UserId == 0 || dto.TopicId == 0)
            {
                return new ApiResponse<Post>(false, "UserId và TopicId không hợp lệ.", null);
            }

            string imageUrl = null;
            try
            {
                if (dto.Image != null)
                {
                    imageUrl = await _photoService.UploadImageAsync(dto.Image);
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        return new ApiResponse<Post>(false, "Lỗi tải ảnh lên, không có URL trả về.", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<Post>(false, "Lỗi khi tải ảnh lên: " + ex.Message, null);
            }

            try
            {
                var post = new Post
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    ImageUrl = imageUrl, // Lưu URL ảnh (nếu có)
                    UserId = dto.UserId,
                    TopicId = dto.TopicId,
                    //IsApproved = false,
                    //IsLocked = false,
                    CreatedAt = DateTime.Now
                };

                // Lưu bài viết vào cơ sở dữ liệu
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
                return new ApiResponse<string>(false, "Không tìm thấy bài viết.");

            if (post.isAproved)
                return new ApiResponse<string>(false, "Bài viết đã được duyệt.");

            post.isAproved = true;
            await _postRepo.UpdateAsync(post);

            // Gửi thông báo cho người tạo bài viết
            var senderId = 1011;
            var receiverId = post.UserId;
            var content = $"🎉 Bài viết \"{post.Title}\" của bạn đã được duyệt!";

            await _notificationService.SendNotificationAsync(
                senderId,
                receiverId,
                content,
                NotificationType.System
            );

            return new ApiResponse<string>(true, "Bài viết của bạn đã được duyệt.");
        }


        public async Task<ApiResponse<object>> LikePostAsync(int blogId, int userId)
        {
            var blog = await _postRepo.GetByIdAsync(blogId);
            if (blog == null)
                return new ApiResponse<object>(false, "Blog not found");

            // Kiểm tra xem blog có danh sách likes chưa, nếu chưa thì khởi tạo
            blog.Likes ??= new List<int>();

            bool isLiked;
            if (blog.Likes.Contains(userId))
            {
                blog.Likes.Remove(userId); // Xóa like của user
                isLiked = false;
            }
            else
            {
                blog.Likes.Add(userId); // Thêm like của user
                isLiked = true;
            }

            // Cập nhật blog với danh sách like mới
            await _postRepo.UpdateAsync(blog);

            // Trả về thông báo cùng với isLiked và likeCount
            return new ApiResponse<object>(true, isLiked ? "Liked blog" : "Unliked blog", new
            {
                isLiked = isLiked,
                likeCount = blog.Likes.Count
            });
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
        public async Task<PagedResult<PostDto>> GetPagedBlogsAdminNotApprovedAsync(int page, int pageSize, int? currentUserId)
        {
            return await _postRepo.GetPagedBlogsAdminNotApprovedAsync(page, pageSize, currentUserId);
        }
        public async Task<PagedResult<PostDto>> GetPagedBlogswithUserIDAsync(int page, int pageSize, int? currentUserId)
        {
            return await _postRepo.GetPagedBlogsUseridAsync(page, pageSize, currentUserId);
        }


    }
}
