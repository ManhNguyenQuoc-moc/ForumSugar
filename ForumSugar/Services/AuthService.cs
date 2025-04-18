
using ForumSugar.DTOs;
using ForumSugar.Helpers;
using ForumSugar.Models.Entities;
using ForumSugar.Repositories;
using ForumSugar.Services.Interfaces;
using global::ForumSugar.Helpers;
using global::ForumSugar.Models.Entities;
using global::ForumSugar.Repositories;
using global::ForumSugar.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumSugar.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool?> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.GetByUserNameAsync(dto.UserName) == null && await _userRepository.EmailExistsAsync(dto.Email) == false)
            {
                var user = new User
                {
                    Name = dto.Name,
                    Username = dto.UserName,
                    Email = dto.Email,
                    PasswordHash = PasswordHasher.Hash(dto.Password),
                    Role = "User",
                    PhoneNumber = dto.PhoneNumber,
                    Bornday = dto.Bornday,
                    AvatarUrl = "https://upanh123.com/wp-content/uploads/2020/11/hinh-anh-con-meo-cute9.jpg",
                    Bio = "Chưa cập nhật",
                    IsLocked = false
                };
                await _userRepository.AddAsync(user);
                var saved = await _userRepository.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUserNameAsync(dto.UserName);
            if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
                return null;
            return JwtHelper.GenerateToken(user);
        }
    }
}


