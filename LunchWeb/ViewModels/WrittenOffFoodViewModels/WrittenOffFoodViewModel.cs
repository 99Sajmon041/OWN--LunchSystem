using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.WrittenOffFoodViewModels
{
    public class WrittenOffFoodViewModel
    {
        [Display(Name = "#")]
        public int Id { get; set; }

        [Display(Name = "Jméno zákazníka")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Role")]
        public string RoleName { get; set; } = string.Empty;

        [Display(Name = "Název jídla")]
        public string Foodname { get; set; } = null!;

        [Display(Name = "Varianta")]
        public int LunchOption { get; set; }

        [Display(Name = "Čas čipnutí")]
        public DateTime TimeOfCreate { get; set; }

        [Display(Name = "Datum")]
        public DateOnly Date => DateOnly.FromDateTime(TimeOfCreate);

        [Display(Name = "Cena")]
        public decimal FinalPrice { get; set; }
    }
}
