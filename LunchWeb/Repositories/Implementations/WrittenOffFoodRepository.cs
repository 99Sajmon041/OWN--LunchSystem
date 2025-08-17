using Lunch.Domain.Models;
using Lunch.Infrastructure.Database;
using LunchWeb.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LunchWeb.Repositories.Implementations
{
    public class WrittenOffFoodRepository : GenericsRepository<WrittenOffFood>, IWrittenOffFoodRepository
    {
        public WrittenOffFoodRepository(ApplicationDbContext context) : base(context)
        {

        }

        public IQueryable<WrittenOffFood> GetAll()
        {
            return dbSet.Include(x => x.Customer)
                .Include(x => x.DailyOffer)
                .ThenInclude(offer => offer!.Meal)
                .AsQueryable();
        }

        public async Task<WrittenOffFood?> GetByIdAsync(int id)
        {
            return await dbSet.Include(x => x.Customer)
                .Include(x => x.DailyOffer)
                .ThenInclude(offer => offer!.Meal)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
