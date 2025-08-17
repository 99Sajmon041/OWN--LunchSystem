using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.AccountViewModels
{
    public class TopUpCreditViewModel
    {
        [Display(Name = "ID uživatele:")]
        public string Id { get; set; } = null!;

        [Display(Name = "Celé jméno uživatele:")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Částka dobití kreditu:")]
        [Required(ErrorMessage = "Částka je povinná!")]
        [Range(100, 5000, ErrorMessage = "Částka musí být v rozmezí 100–5000 Kč.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
    }
}
