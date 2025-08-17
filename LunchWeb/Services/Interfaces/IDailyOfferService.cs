using LunchWeb.ViewModels.DailyOfferViewModels;

namespace LunchWeb.Services.Interfaces
{
    public interface IDailyOfferService
    {
        Task<DailyOfferWeeklyViewModel> GetWeeklyOfferAsync(DateOnly weekStart);
        Task SaveWeeklyOfferAsync(DailyOfferWeeklyViewModel dailyOfferViewModel);
    }
}
