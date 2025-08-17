namespace LunchWeb.ViewModels.DailyOfferViewModels
{
    public class DailyOfferWeeklyViewModel
    {
        public DateOnly StartOfWeek { get; set; }
        public DateOnly EndOfWeek => StartOfWeek.AddDays(6);

        public bool CanEditEntireWeek { get; set; }
        public DateOnly LockedWeekStart { get; set; }
        public DateOnly FirstEditableWeekStart { get; set; }
        public bool CanGoBack { get; set; }
        public bool IsCurrentWeek { get; set; }

        public List<DailyOfferDayItem> ItemsPerDay { get; set; } = new();
    }
}
