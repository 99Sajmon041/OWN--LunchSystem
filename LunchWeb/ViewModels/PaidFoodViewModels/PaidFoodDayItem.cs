using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.PaidFoodViewModels
{
    public class PaidFoodDayItem
    {
        [Display(Name = "Datum")]
        public DateOnly Date { get; set; }

        [Display(Name = "Vybrané jídlo")]
        public int? SelectedDailyOfferId { get; set; }

        [BindNever]
        [ValidateNever]
        public SelectList Options { get; set; } = default!;

        public List<MealOptionVm> MealOptions { get; set; } = new();
    }
}
