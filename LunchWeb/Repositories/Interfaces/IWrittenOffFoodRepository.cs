using Lunch.Domain.Models;

namespace LunchWeb.Repositories.Interfaces
{
    public interface IWrittenOffFoodRepository : IGenericsRepository<WrittenOffFood>
    {
        IQueryable<WrittenOffFood> GetAll();
        Task<WrittenOffFood?> GetByIdAsync(int id);
    }
}
