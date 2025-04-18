using ForumSugar.Data;
using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Models.Responses;
using ForumSugar.Repositories;
using ForumSugar.Repositories.Interfaces;
using ForumSugar.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSugar.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepo;
       
        private readonly ICommentReportRepository _reportRepo;

        public CommentService(ICommentRepository commentRepo,
                              
                                   ICommentReportRepository reportRepo)
        {
            _commentRepo = commentRepo;
         
            _reportRepo = reportRepo;
        }

        public async Task<ApiResponse<Comment>> AddCommentAsync(CreateCommentDto dto)
        {
            var comment = new Comment
            {
                Content = dto.Content,
                PostId = dto.BlogId,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepo.AddAsync(comment);
            return new ApiResponse<Comment>(true, "Bình luận thành công", comment);
        }

        public async Task<ApiResponse<string>> LikeCommentAsync(int commentId, int userId)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId);
            if (comment == null)
                return new ApiResponse<string>(false, "Comment not found");

            // Khởi tạo danh sách Likes nếu chưa có
            comment.Likes ??= new List<int>();

            // Kiểm tra xem người dùng đã like comment chưa
            if (comment.Likes.Contains(userId))
            {
                // Nếu đã like, bỏ like (unlike)
                comment.Likes.Remove(userId);
                await _commentRepo.UpdateAsync(comment);
                return new ApiResponse<string>(true, "Unliked comment");
            }
            else
            {
                // Nếu chưa like, thêm like
                comment.Likes.Add(userId);
                await _commentRepo.UpdateAsync(comment);
                return new ApiResponse<string>(true, "Liked comment");
            }
        }


        public async Task<ApiResponse<IEnumerable<Comment>>> GetCommentsByBlogIdAsync(int postId)
        {
            try
            {
                // Lấy danh sách các bình luận cho blog với các include là Replies, Likes và User
                var comments = await _commentRepo.GetCommentByBlogIdAsync(
                    x => x.PostId == postId,
                    includes: new[] { "Replies", "Likes", "User" }
                );

                // Kiểm tra nếu không có bình luận nào
                if (comments == null || !comments.Any())
                {
                    return new ApiResponse<IEnumerable<Comment>>(false, "Không có bình luận nào cho bài viết này.", null);
                }

                // Trả về ApiResponse với kết quả thành công
                return new ApiResponse<IEnumerable<Comment>>(true, "Lấy bình luận thành công", comments);
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, trả về ApiResponse với thông báo lỗi
                return new ApiResponse<IEnumerable<Comment>>(false, $"Có lỗi xảy ra: {ex.Message}", null);
            }
        }

        public async Task<ApiResponse<string>> ReportCommentAsync(CreateCommentReportDto dto)
        {
            var report = new CommentReport
            {
                CommentId = dto.CommentId,
                Reason = dto.Reason,
                UserId = dto.UserId,
                ReportedAt = DateTime.UtcNow
            };

            await _reportRepo.AddAsync(report);
            return new ApiResponse<string>(true, "Báo cáo bình luận thành công", null);
        }
    }

}
