using Microsoft.AspNetCore.Mvc.Rendering;

namespace LunchWeb.ViewModels.DailyOfferViewModels
{
    public class DailyMealItem
    {
        public int? MealId { get; set; }
        public SelectList? MealOptions { get; set; }
    }
}
