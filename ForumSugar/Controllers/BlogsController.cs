using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Models.Responses;
using ForumSugar.Services.Interfaces;
using ForumSugar.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ForumSugar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IPostService _postService;

        public BlogsController(IPostService blogService)
        {
            _postService = blogService;
        }

        // GET: api/Blogs
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Post>>>> GetAll()
        {
            var response = await _postService.GetAllAsync();
            if (response.Success)
                return Ok(response);  // Trả về 200 OK và kết quả nếu thành công
            return BadRequest(response);  // Trả về 400 BadRequest nếu có lỗi
        }

        // GET: api/Blogs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Post>>> GetById(int id)
        {
            var response = await _postService.GetByIdAsync(id);
            if (response.Success)
                return Ok(response);
            return NotFound(response);  // Trả về 404 nếu không tìm thấy blog
        }

        // POST: api/Blogs
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Post>>> Create([FromBody] CreatePostDto dto)
        {
            if (dto == null)
                return BadRequest("Post cannot be null.");
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //dto.UserId = userId;
            var response = await _postService.CreateAsync(dto);
            if (response.Success)
                return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
            return BadRequest(response);  // Trả về 400 nếu có lỗi khi tạo blog
        }
        // PUT: api/Blogs/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<Post>>> Update(int id, [FromBody] Post blog)
        {
            if (blog == null)
                return BadRequest("Post cannot be null.");

            var response = await _postService.UpdateAsync(id, blog);
            if (response.Success)
                return Ok(response);
            return NotFound(response);  // Trả về 404 nếu không tìm thấy blog để cập nhật
        }
        // DELETE: api/Blogs/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var response = await _postService.DeleteAsync(id);
            if (response.Success)
                return Ok(response);
            return NotFound(response);  // Trả về 404 nếu không tìm thấy blog để xóa
        }
        [HttpPost("like/{blogId}")]
        public async Task<ApiResponse<string>> LikeBlogAsync(int blogId, [FromQuery] int userId)
        {
            return await _postService.LikePostAsync(blogId, userId);
        }
        // GET: api/Blogs/paged?pageNumber=1&pageSize=10
        //[HttpGet("paged")]
        //public async Task<ActionResult<ApiResponse<PagedResult<Post>>>> GetPagedPosts(
        //     [FromQuery] int pageNumber = 1,
        //     [FromQuery] int pageSize = 10,
        //     [FromQuery] string? topic = null)
        //{
        //    var response = await _postService.GetPagedPostsAsync(pageNumber, pageSize, topic);
        //    if (!response.Success)
        //        return BadRequest(response.Message);

        //    return Ok(response);  // Trả về dữ liệu bài viết với phân trang
        //}
        [HttpGet("scroll")]
        public async Task<IActionResult> GetBlogsScroll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, int? currentUserId = null)
        {
            var result = await _postService.GetPagedBlogsAsync(page, pageSize, currentUserId);
            return Ok(result);
        }
        //[HttpGet("scroll")]
        //public async Task<IActionResult> GetBlogsScrollwithTopic([FromQuery] int page = 1, [FromQuery] int pageSize = 10, int? currentUserId = null)
        //{
        //    var result = await _postService.GetPagedBlogsAsync(page, pageSize, currentUserId);
        //    return Ok(result);
        //}
        //[HttpGet("scroll")]
        //public async Task<IActionResult> GetBlogsScrollwithMakert([FromQuery] int page = 1, [FromQuery] int pageSize = 10, int? currentUserId = null)
        //{
        //    var result = await _postService.GetPagedBlogsAsync(page, pageSize, currentUserId);
        //    return Ok(result);
        //}
        //[HttpGet("scroll")]//xem bài viết của bản thân đã được duyệt chưa duyệt 
        //public async Task<IActionResult> GetBlogsScrollwithUserID([FromQuery] int page = 1, [FromQuery] int pageSize = 10, int? currentUserId = null)
        //{
        //    var result = await _postService.GetPagedBlogsAsync(page, pageSize, currentUserId);
        //    return Ok(result);
        //}
        //[HttpGet("scroll")]//xem danh sách bài viết được lưu
        //public async Task<IActionResult> GetBlogsScrollwithSavePost([FromQuery] int page = 1, [FromQuery] int pageSize = 10, int? currentUserId = null)
        //{
        //    var result = await _postService.GetPagedBlogsAsync(page, pageSize, currentUserId);
        //    return Ok(result);
        //}
        //[HttpGet("scroll")]//trang dành cho admin duyệt 
        //public async Task<IActionResult> GetBlogsScrollwithNoIproved([FromQuery] int page = 1, [FromQuery] int pageSize = 10, int? currentUserId = null)
        //{
        //    var result = await _postService.GetPagedBlogsAsync(page, pageSize, currentUserId);
        //    return Ok(result);
        //}
        //[HttpGet("scroll")]//trang dành cho admin xem các bài viết bị report
        //public async Task<IActionResult> GetBlogsScrollwithReport([FromQuery] int page = 1, [FromQuery] int pageSize = 10, int? currentUserId = null)
        //{
        //    var result = await _postService.GetPagedBlogsAsync(page, pageSize, currentUserId);
        //    return Ok(result);
        //}

    }
}
