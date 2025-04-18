using ForumSugar.Data;
using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Models.Responses;
using ForumSugar.Repositories;
using ForumSugar.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumSugar.Services
{
    public class TopicService : ITopicService
    {
        private readonly IGenericRepository<Topic> topicRepo;

        public TopicService(IGenericRepository<Topic> _topicRepo)
        {
            topicRepo = _topicRepo;
        }

        public async Task<ApiResponse<IEnumerable<Topic>>> GetAllAsync()
        {
            var topics = await topicRepo.GetAllAsync();
            return new ApiResponse<IEnumerable<Topic>>(true, "Lấy danh sách chủ đề thành công", topics);
        }

        public async Task<ApiResponse<Topic>> GetByIdAsync(int id)
        {
            var topic = await topicRepo.GetByIdAsync(id);
            if (topic == null)
                return new ApiResponse<Topic>(false, "Không tìm thấy chủ đề");

            return new ApiResponse<Topic>(true, "Lấy thông tin chủ đề thành công", topic);
        }

        public async Task<ApiResponse<Topic>> CreateAsync(TopicCreateDto dto)
        {
            try
            {
                // Chuyển DTO sang entity
                var topic = new Topic
                {
                    Name = dto.name,
                };

                await topicRepo.AddAsync(topic); // 👍 đúng kiểu entity
                await topicRepo.SaveChangesAsync();

                return new ApiResponse<Topic>(true, "Tạo chủ đề thành công", topic);
            }
            catch (Exception ex)
            {
                return new ApiResponse<Topic>(false, "Lỗi khi tạo chủ đề: " + ex.Message);
            }
        }

        public async Task<ApiResponse<Topic>> UpdateAsync(int id, Topic topic)
        {
            var existing = await topicRepo.GetByIdAsync(id);
            if (existing == null)
                return new ApiResponse<Topic>(false, "Không tìm thấy chủ đề");

            existing.Name = topic.Name;

            try
            {
                await topicRepo.SaveChangesAsync();
                return new ApiResponse<Topic>(true, "Cập nhật chủ đề thành công", existing);
            }
            catch (Exception ex)
            {
                return new ApiResponse<Topic>(false, "Lỗi khi cập nhật chủ đề: " + ex.Message);
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            // Sử dụng phương thức GetWithIncludeAsync để lấy chủ đề cùng các bài viết của nó
            var topic = await topicRepo.GetWithIncludeAsync(t => t.Id == id, t => t.Posts);

            if (topic == null)
            {
                return new ApiResponse<string>(false, "Chủ đề không tồn tại.");
            }

            // Kiểm tra nếu chủ đề có bài viết
            if (topic.Posts != null && topic.Posts.Any())
            {
                return new ApiResponse<string>(false, "Chủ đề này có bài viết, không thể xóa.");
            }

            // Xóa chủ đề nếu không có bài viết
            topicRepo.DeleteAsync(topic);
            try
            {
                await topicRepo.SaveChangesAsync();
                return new ApiResponse<string>(true, "Xóa chủ đề thành công.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, $"Lỗi khi xóa: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Topic>> GetTopicWithPostsAsync(int id)
        {
            // Sử dụng phương thức GetWithIncludeAsync để lấy chủ đề cùng các bài viết của nó
            var topic = await topicRepo.GetWithIncludeAsync(t => t.Id == id, t => t.Posts);
            if (topic == null)
            {
                return new ApiResponse<Topic>(false, "Chủ đề không tồn tại.");
            }
            return new ApiResponse<Topic>(true, "Chủ đề với bài viết.", topic);
        }


    }
}
