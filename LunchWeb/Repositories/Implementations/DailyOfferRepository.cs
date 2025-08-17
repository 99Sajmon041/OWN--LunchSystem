using Lunch.Domain.Models;
using Lunch.Infrastructure.Database;
using LunchWeb.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LunchWeb.Repositories.Implementations
{
    public class DailyOfferRepository : GenericsRepository<DailyOffer>, IDailyOfferRepository
    {
        public DailyOfferRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<DailyOffer> GetAll()
        {
            return dbSet.Include(x => x.Meal).AsQueryable();
        }

        public async Task<DailyOffer?> GetByIdAsync(int id)
        {
            return await dbSet.Include(x => x.Meal).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> CountOffersAsync(DateOnly from, DateOnly to, CancellationToken ct)
        {
            return await dbSet
                .Where(x => x.Date >= from && x.Date <= to && x.Position >= 0 && x.Position <= 2)
                .Select(x => new { Day = x.Date, x.Position })
                .Distinct()
                .CountAsync(ct);
        }
    }
}
