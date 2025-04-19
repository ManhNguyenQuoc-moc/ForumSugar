using ForumSugar.Data;
using ForumSugar.DTOs;
using ForumSugar.Models.Entities;
using ForumSugar.Repositories;
using ForumSugar.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumSugar.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> UpdateProfileAsync(int id, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            user.Name = dto.FullName;
            user.Bio = dto.Bio;
            user.AvatarUrl = dto.AvatarUrl;

            await _userRepository.SaveChangesAsync();
            return user;
        }
        public async Task<UserProfileDto?> GetProfileAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var dto = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl
            };

            return dto;
        }
        public async Task<List<UserListDto>> GetAllUsersAsync()
        {
            var userList = await _userRepository.GetAllAsync();
            var userDtos = userList.Select(u => new UserListDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            }).ToList();
            return userDtos;
        }

        public async Task<User> SetUserLockStatusAsync(int id, bool isLocked)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.IsLocked = isLocked;
            await _userRepository.SaveChangesAsync();

            return user;
        }
        public async Task<User> UpdateUserRoleAsync(int id, string newRole)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.Role = newRole;
            await _userRepository.SaveChangesAsync();

            return user;
        }
        public async Task<bool> PermanentlyDeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;
            _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }


    }
}
