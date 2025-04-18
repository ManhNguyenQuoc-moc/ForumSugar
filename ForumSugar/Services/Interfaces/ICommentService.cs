using ForumSugar.DTOs;
using ForumSugar.Models.Responses;

namespace ForumSugar.Services.Interfaces
{
    public interface ICommentService
    {
        Task<ApiResponse<Comment>> AddCommentAsync(CreateCommentDto dto);
        Task<ApiResponse<string>> LikeCommentAsync(int commentId, int userId);
        Task<ApiResponse<IEnumerable<Comment>>> GetCommentsByBlogIdAsync(int blogId);
        Task<ApiResponse<string>> ReportCommentAsync(CreateCommentReportDto dto);
    }
}
