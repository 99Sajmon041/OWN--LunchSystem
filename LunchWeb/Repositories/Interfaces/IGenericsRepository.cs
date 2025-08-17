namespace LunchWeb.Repositories.Interfaces
{
    public interface IGenericsRepository<TEntity> where TEntity : class
    {
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task CreateRangeAsync(IEnumerable<TEntity> items);
        Task UpdateRangeAsync(IEnumerable<TEntity> items);
        Task RemoveRangeAsync(IEnumerable<TEntity> items);
    }
}
