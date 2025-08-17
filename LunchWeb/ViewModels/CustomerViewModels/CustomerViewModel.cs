using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.CustomerViewModels
{
    public class CustomerViewModel
    {
        [Display(Name = "ID")]
        public string Id { get; set; } = null!;

        [Display(Name = "Křestní jméno")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Příjmení")]
        public string LastName { get; set; } = null!;

        [Display(Name = "E-mail")]
        public string Email { get; set; } = null!;

        [Display(Name = "Datum narození")]
        public DateOnly DateOfBirth { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; } = null!;

        [Display(Name = "Dospělý")]
        public bool IsAdult { get; set; }

        [Display(Name = "Aktivní účet")]
        public bool Active { get; set; }

        [Display(Name = "Kredit")]
        public decimal Credit { get; set; }

        [Display(Name = "ID karty")]
        public string CardId { get; set; } = null!;
    }
}
