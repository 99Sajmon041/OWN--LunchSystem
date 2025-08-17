using LunchWeb.Helpers;
using LunchWeb.ViewModels.DiscountViewModels;

namespace LunchWeb.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<IReadOnlyList<DiscountViewModel>> GetAllAsync();
        Task<UpdateResult> UpdateAsync(IEnumerable<DiscountViewModel> updates);
        Task<decimal> GetByroleName(string roleName);
    }
}
