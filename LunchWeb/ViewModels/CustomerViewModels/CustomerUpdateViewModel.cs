using Lunch.Domain.Enums;
using LunchWeb.Helpers.CustomValidators;
using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.CustomerViewModels
{
    public class CustomerUpdateViewModel
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Zadejte jméno!")]
        [Display(Name = "Jméno")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Zadejte příjmení!")]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Zadejte E-Mail!")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Zadejte datum narození!")]
        [DataType(DataType.Date)]
        [BirthDateAttribute]
        [Display(Name = "Datum narození")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vyberte roli!")]
        [Display(Name = "Role")]
        public Role Role { get; set; }

        [Required(ErrorMessage = "Zadejte ID karty - naskenujte kartu!")]
        [Display(Name = "ID karty")]
        public string CardId { get; set; } = null!;

        [Display(Name = "Aktivní účet")]
        public bool Active { get; set; }

        [Display(Name = "Vynutit změnu hesla")]
        public bool MustChangePassword { get; set; }
    }
}
