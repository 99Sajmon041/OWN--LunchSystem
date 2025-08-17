using LunchWeb.ViewModels.MealViewModels;

namespace LunchWeb.Services.Interfaces
{
    public interface IMealService
    {
        Task<List<MealViewModel>> GetAllMealsAsync();
        Task<MealViewModel?> GetMealByIdAsync(int id);
        Task CreateMealAsync(MealFormViewModel mealFormViewModel);
        Task UpdateMealAsync(MealFormViewModel mealFormViewModel);
        Task DeleteMealAsync(int id);
    }
}
