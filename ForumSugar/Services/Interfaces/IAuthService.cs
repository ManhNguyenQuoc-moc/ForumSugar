using ForumSugar.DTOs;
using ForumSugar.Models.Entities;

namespace ForumSugar.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool?> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
