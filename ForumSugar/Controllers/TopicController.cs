using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Models.Responses;
using ForumSugar.Services;
using ForumSugar.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ForumSugar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }
        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Post>>> Create([FromBody] TopicCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Post cannot be null.");
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //dto.UserId = userId;
            var response = await _topicService.CreateAsync(dto);
            if (response.Success)
                return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
            return BadRequest(response);  // Trả về 400 nếu có lỗi khi tạo blog
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Topic>>> GetById(int id)
        {
            var response = await _topicService.GetByIdAsync(id);
            if (response.Success)
                return Ok(response);
            return NotFound(response);
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Topic>>>> GetAll()
        {
            var response = await _topicService.GetAllAsync();
            return Ok(response);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Topic>>> Update(int id, [FromBody] Topic topic)
        {
            var response = await _topicService.UpdateAsync(id, topic);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
        // Phương thức lấy thông tin chủ đề với bài viết
        [HttpGet("{id}/with-posts")]
        public async Task<IActionResult> GetTopicWithPosts(int id)
        {
            var response = await _topicService.GetTopicWithPostsAsync(id);

            if (!response.Success)
            {
                return NotFound(response.Message);  // Trả về lỗi 404 nếu không tìm thấy chủ đề
            }

            return Ok(response);  // Trả về thông tin chủ đề nếu thành công
        }

        // Phương thức xóa chủ đề
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var response = await _topicService.DeleteAsync(id);

            if (!response.Success)
            {
                return BadRequest(response.Message);  // Trả về lỗi 400 nếu không thể xóa
            }

            return Ok(response);  // Trả về thông báo thành công
        }
    }

}

