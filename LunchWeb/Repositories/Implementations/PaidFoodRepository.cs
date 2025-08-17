using Lunch.Domain.Models;
using Lunch.Infrastructure.Database;
using LunchWeb.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LunchWeb.Repositories.Implementations
{
    public class PaidFoodRepository : GenericsRepository<PaidFood>, IPaidFoodRepository
    {
        public PaidFoodRepository(ApplicationDbContext context) : base(context)
        {

        }

        public IQueryable<PaidFood> GetAll()
        {
            return dbSet.Include(x => x.Customer).Include(x => x.DailyOffer).AsQueryable();
        }

        public async Task<PaidFood?> GetByIdAsync(int id)
        {
            return await dbSet.Include(x => x.Customer)
                .Include(x => x.DailyOffer)
                .ThenInclude(offer => offer.Meal)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
