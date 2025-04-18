using ForumSugar.DTOs;
using ForumSugar.Helpers;
using ForumSugar.Services;
using ForumSugar.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ForumSugar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfileFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Không tìm thấy thông tin người dùng trong token");
            var userId = int.Parse(userIdClaim.Value);
            var profile = await _userService.GetProfileAsync(userId);
            if (profile == null) return NotFound();

            return Ok(profile);
        }
        // PUT: api/users/{id}
        //[Authorize]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateProfile( int userId, UpdateProfileDto dto)
        {
            //var userId = int.Parse(User.FindFirst("id").Value);
            var user = await _userService.UpdateProfileAsync(userId, dto);
            if (user == null) return NotFound();
            return Ok(new { message = "Thông tin đã được cập nhật." });
        }
        [HttpGet("users")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> GetAllUser()
        {
            var userslist = await _userService.GetAllUsersAsync();
            if (userslist == null || !userslist.Any())
                return NotFound("Không tìm thấy người dùng nào.");

            return Ok(userslist);
        }
        [HttpPut("users")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoleUser(int id)
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != "Admin")
                return Forbid("Bạn không có quyền truy cập!");
            var userslist = await _userService.GetAllUsersAsync();
            if (userslist == null || !userslist.Any())
                return NotFound("Không tìm thấy người dùng nào.");

            return Ok(userslist);
        }

    }
}
