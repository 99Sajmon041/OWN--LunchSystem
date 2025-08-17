using Lunch.Domain.Models;

namespace LunchWeb.Repositories.Interfaces
{
    public interface IPaidFoodRepository : IGenericsRepository<PaidFood>
    {
        IQueryable<PaidFood> GetAll();
        Task<PaidFood?> GetByIdAsync(int id);
    }
}
