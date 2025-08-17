using LunchWeb.Helpers;
using LunchWeb.Repositories.Interfaces;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.DiscountViewModels;

namespace LunchWeb.Services.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<IReadOnlyList<DiscountViewModel>> GetAllAsync()
        {
            var discounts = await _discountRepository.GetAllForUiAsync();
            return discounts.Select(d => new DiscountViewModel
            {
                RoleId = d.roleId,
                RoleName = d.roleName,
                Discount = Math.Round((d.discount ?? 0m) * 100m, 0, MidpointRounding.AwayFromZero)
            })
            .ToList();
        }

        public async Task<UpdateResult> UpdateAsync(IEnumerable<DiscountViewModel> updates)
        {
            var affected = await _discountRepository.UpdateManyAsync(updates.Select(u => (u.RoleId, discount: Math.Clamp(u.Discount, 0, 100) / 100m)));
            return affected > 0 ? UpdateResult.Updated : UpdateResult.NoChanges;
        }

        public async Task<decimal> GetByroleName(string roleName)
        {
            return await _discountRepository.GetByRoleNameAsync(roleName);
        }
    }
}
