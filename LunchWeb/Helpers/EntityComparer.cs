using Lunch.Domain.Models;
using LunchWeb.ViewModels.CustomerViewModels;
using LunchWeb.ViewModels.MealViewModels;

namespace LunchWeb.Helpers
{
    public class EntityComparer
    {
        public static bool IsCustomerSame(Customer e, CustomerUpdateViewModel vm)
        {
            return string.Equals(vm.FirstName.Trim(), (e.FirstName ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(vm.LastName.Trim(), (e.LastName ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(vm.Email.Trim(), (e.Email ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                && vm.DateOfBirth == e.DateOfBirth
                && vm.Role == e.Role
                && string.Equals(vm.CardId.Trim(), (e.CardId ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                && vm.Active == e.Active
                && vm.MustChangePassword == e.MustChangePassword;
        }

        public static bool IsMealSame(Meal e, MealFormViewModel vm)
        {
            return string.Equals(vm.FoodName.Trim(), (e.FoodName ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                && string.Equals(vm.FoodDescription.Trim(), (e.FoodDescription ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase)
                && decimal.Round(vm.Price, 2) == decimal.Round(e.Price, 2);
        }
    }
}
