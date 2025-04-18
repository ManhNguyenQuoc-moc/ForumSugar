using ForumSugar.Models.Entities;
namespace ForumSugar.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetByUserNameAsync(string ussername);
    }

}
