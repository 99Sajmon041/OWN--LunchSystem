using Lunch.Infrastructure.Database;
using LunchWeb.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LunchWeb.Repositories.Implementations
{
    public class GenericsRepository<TEntity> : IGenericsRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> dbSet;
        public GenericsRepository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            dbSet.Remove(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            dbSet.Update(entity);
            await SaveChangesAsync();
        }

        protected async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<TEntity> items)
        {
            await dbSet.AddRangeAsync(items);
            await SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> items)
        {
            dbSet.UpdateRange(items);
            await SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<TEntity> items)
        {
            dbSet.RemoveRange(items);
            await SaveChangesAsync();
        }
    }
}