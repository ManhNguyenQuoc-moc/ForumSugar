using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Models.Responses;
using ForumSugar.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForumSugar.Services.Interfaces
{
    public interface IPostService
    {
        Task<ApiResponse<IEnumerable<Post>>> GetAllAsync();
        Task<ApiResponse<Post>> GetByIdAsync(int id);
        Task<ApiResponse<Post>> CreateAsync(CreatePostDto dto);
        Task<ApiResponse<Post>> UpdateAsync(int id, Post Post);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<Post>>> GetPostsByUserIdAsync(int userId);
        Task<ApiResponse<IEnumerable<Post>>> SearchPostsAsync(string keyword);
        //Task<ApiResponse<PagedResult<Post>>> GetPagedPostsAsync(int pageNumber, int pageSize, string? topic = null);
        Task<ApiResponse<string>> ApprovePostAsync(int PostId);
        Task<ApiResponse<object>> LikePostAsync(int blogId, int userId);
        Task<PagedResult<PostDto>> GetPagedBlogsAsync(int page, int pageSize, int? currentUserId);
        Task<PagedResult<PostDto>> GetPagedBlogsAdminNotApprovedAsync(int page, int pageSize, int? currentUserId = null);
        Task<PagedResult<PostDto>> GetPagedBlogswithUserIDAsync(int page, int pageSize, int? currentUserId = null);

    }
}