using ForumSugar.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ForumSugar.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return await SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return await SaveChangesAsync();
        }
 
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<T> GetWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Áp dụng điều kiện filter
            query = query.Where(predicate);

            return await query.FirstOrDefaultAsync();
        }
        public IQueryable<T> GetAllAsQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }


}
