using Lunch.Domain.Models;

namespace LunchWeb.Repositories.Interfaces
{
    public interface IDailyOfferRepository : IGenericsRepository<DailyOffer>
    {
        IQueryable<DailyOffer> GetAll();
        Task<DailyOffer?> GetByIdAsync(int id);
        Task<int> CountOffersAsync(DateOnly from, DateOnly to, CancellationToken ct);
    }
}
