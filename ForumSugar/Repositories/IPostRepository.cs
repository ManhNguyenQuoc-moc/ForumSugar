using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Wrappers;

namespace ForumSugar.Repositories.Interfaces
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<IEnumerable<Post>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Post>> SearchAsync(string keyword);
        Task<PagedResult<PostDto>> GetPagedAsync(int page, int pageSize, int? currentUserId = null);
        Task<int> CountAsync();
        Task<PagedResult<PostDto>> GetPagedBlogsAdminNotApprovedAsync(int page, int pageSize, int? currentUserId = null);
        Task<PagedResult<PostDto>> GetPagedBlogsUseridAsync(int page, int pageSize, int? currentUserId = null);
    }
}
