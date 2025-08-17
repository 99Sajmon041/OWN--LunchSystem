namespace LunchWeb.ViewModels.DailyOfferViewModels
{
    public class DailyOfferDayItem
    {
        public DateOnly Date { get; set; }
        public List<DailyMealItem> Meals { get; set; } = new();
    }
}