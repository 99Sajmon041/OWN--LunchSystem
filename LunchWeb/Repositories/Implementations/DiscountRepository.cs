using Lunch.Infrastructure.Database;
using LunchWeb.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LunchWeb.Repositories.Implementations
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _db;

        public DiscountRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<(string roleId, string roleName, decimal? discount)>> GetAllForUiAsync()
        {
            var data = await (from r in _db.Roles
                              join d in _db.RoleDiscounts
                              on r.Id equals d.RoleId into discounts
                              from d in discounts.DefaultIfEmpty()
                              orderby r.Name
                              select new { r.Id, r.Name, Discount = (decimal?)d.Discount })
                              .AsNoTracking()
                              .ToListAsync();

            return data.Select(x => (x.Id, x.Name, x.Discount)).ToList();
        }


        public async Task<int> UpdateManyAsync(IEnumerable<(string roleId, decimal discount)> updates)
        {
            var updateList = updates.ToList();
            var roleIds = updateList.Select(u => u.roleId).Distinct().ToList();
            var existingRoleIds = await _db.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync();

            var validUpdates = updateList
                .Where(u => existingRoleIds.Contains(u.roleId))
                .ToList();

            if (validUpdates.Count == 0) return 0;

            var existing = await _db.RoleDiscounts
                .Where(rd => existingRoleIds.Contains(rd.RoleId))
                .ToListAsync();

            var map = existing.ToDictionary(x => x.RoleId);
            int affected = 0;

            foreach (var (roleId, discount) in validUpdates)
            {
                if (map.TryGetValue(roleId, out var rd) && rd.Discount != discount)
                {
                    rd.Discount = discount;
                    affected++;
                }
            }
            await _db.SaveChangesAsync();
            return affected;
        }


        public async Task<decimal> GetByRoleNameAsync(string roleName)
        {
            var discount = await (from r in _db.Roles
                                  join d in _db.RoleDiscounts
                                      on r.Id equals d.RoleId
                                  where r.Name == roleName
                                  select (decimal?)d.Discount)
                                 .FirstOrDefaultAsync();

            return discount ?? 0m;
        }
    }
}
