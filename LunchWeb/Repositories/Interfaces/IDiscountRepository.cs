namespace LunchWeb.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<IReadOnlyList<(string roleId, string roleName, decimal? discount)>> GetAllForUiAsync();
        Task<int> UpdateManyAsync(IEnumerable<(string roleId, decimal discount)> updates);
        Task<decimal> GetByRoleNameAsync(string roleName);
    }
}
