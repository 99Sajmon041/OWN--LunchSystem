using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.PaidFoodViewModels
{
    public class PaidFoodWeeklyViewModel
    {
        [Display(Name = "Začátek týdne")]
        public DateOnly StartOfWeek { get; set; }

        [Display(Name = "Konec týdne")]
        public DateOnly EndOfWeek => StartOfWeek.AddDays(6);
        public bool CanEdit { get; set; }
        public bool CanSave { get; set; }
        public bool CanGoBack { get; set; }
        public DateOnly EditableUntil { get; set; }
        public DateOnly LockedWeekStart { get; set; }
        public DateOnly FirstEditableWeekStart { get; set; }

        [Display(Name = "Nově účtováno")]
        public decimal? PreviewNewCharge { get; set; }

        [Display(Name = "Kredit")]
        public decimal? CurrentBalance { get; set; }
        public IList<PaidFoodDayItem> ItemsPerDay { get; set; } = new List<PaidFoodDayItem>();
    }
}
