using LunchWeb.Repositories.Interfaces;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.WrittenOffFoodViewModels;

namespace LunchWeb.Services.Implementations
{
    public class WrittenOffFoodService : IWrittenOffFoodService
    {
        private readonly IWrittenOffFoodRepository _writtenOffFoodService;

        public WrittenOffFoodService(IWrittenOffFoodRepository writtenOffFoodService)
        {
            _writtenOffFoodService = writtenOffFoodService;
        }

        public IEnumerable<WrittenOffFoodViewModel> GetAll()
        {
            return _writtenOffFoodService.GetAll()
                .Select(w => new WrittenOffFoodViewModel
                {
                    Id = w.Id,
                    FullName = w.Customer.FullName,
                    RoleName = w.Customer.Role.ToString(),
                    Foodname = w.DailyOffer!.Meal.FoodName,
                    LunchOption = w.DailyOffer.Position,
                    TimeOfCreate = w.TimeOfCreate,
                    FinalPrice = w.PriceAfterDiscount
                })
                .ToList();
        }
    }
}
