using System.Linq.Expressions;

namespace ForumSugar.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        IQueryable<T> GetAllAsQueryable();
        Task<bool> SaveChangesAsync();
        Task<T> GetWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);


    }
}
