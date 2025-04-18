using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Models.Responses;

namespace ForumSugar.Services.Interfaces
{
    public interface ITopicService
    {
        Task<ApiResponse<IEnumerable<Topic>>> GetAllAsync();
        Task<ApiResponse<Topic>> GetByIdAsync(int id);
        Task<ApiResponse<Topic>> CreateAsync(TopicCreateDto dto);
        Task<ApiResponse<Topic>> UpdateAsync(int id, Topic topic);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<Topic>> GetTopicWithPostsAsync(int id);
    }
}
