using LunchWeb.ViewModels.WrittenOffFoodViewModels;

namespace LunchWeb.Services.Interfaces
{
    public interface IWrittenOffFoodService
    {
        IEnumerable<WrittenOffFoodViewModel> GetAll();
    }
}
