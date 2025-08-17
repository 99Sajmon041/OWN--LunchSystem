using Lunch.Domain.Models;
using Lunch.Infrastructure.Database;
using LunchWeb.Repositories.Interfaces;

namespace LunchWeb.Repositories.Implementations
{
    public class MealRepository : GenericsRepository<Meal>, IMealRepository
    {
        public MealRepository(ApplicationDbContext context) : base(context)
        {

        }

        public Task<IQueryable<Meal>> GetAllMealsAsync()
        {
            return Task.FromResult(dbSet.AsQueryable());
        }

        public async Task<Meal?> GetMealByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }
    }
}
