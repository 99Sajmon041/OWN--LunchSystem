using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-mail je povinný.")]
        [EmailAddress(ErrorMessage = "Zadejte platný e-mail.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = null!;
    }
}
