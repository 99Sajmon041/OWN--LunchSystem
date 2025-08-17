using LunchWeb.ViewModels.PaidFoodViewModels;

namespace LunchWeb.Services.Interfaces
{
    public interface IPaidFoodService
    {
        Task<PaidFoodWeeklyViewModel> GetWeeklyPaidFoodAsync(string customerId, DateOnly weekStart);
        Task SaveWeeklyPaidFoodAsync(string customerId, PaidFoodWeeklyViewModel viewModel);
        Task<MealOptionVm?> GetMealOptionAsync(int dailyOfferId, string customerId);
    }
}
