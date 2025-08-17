using Lunch.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Lunch.Infrastructure.Database
{
    public partial class DbSeeder
    {
        public static async Task<int> SeedRoleDiscountsAsync(ApplicationDbContext db, decimal defaultDiscount = 0m)
        {
            var allRoles = db.Roles.Select(r => r.Id).ToList();

            if (allRoles.Count == 0)
                return 0;

            var existingRolesId = await db.RoleDiscounts.Select(rd => rd.RoleId).ToListAsync();

            var missingRoles = allRoles.Except(existingRolesId).ToList();
            if (missingRoles.Count == 0)
                return 0;

            foreach (var roleId in missingRoles)
            {
                db.RoleDiscounts.Add(new RoleDiscount
                {
                    RoleId = roleId,
                    Discount = defaultDiscount
                });
            }
            await db.SaveChangesAsync();
            return missingRoles.Count;
        }
    }
}
