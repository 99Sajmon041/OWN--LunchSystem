using Lunch.Domain.Models;

namespace LunchWeb.Repositories.Interfaces
{
    public interface IMealRepository : IGenericsRepository<Meal>
    {
        Task<IQueryable<Meal>> GetAllMealsAsync();
        Task<Meal?> GetMealByIdAsync(int id);
    }
}
