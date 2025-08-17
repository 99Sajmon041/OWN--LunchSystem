using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Zadejte E-mail!")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Zadejte heslo!")]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; } = null!;

        [Display(Name = "Zapamatovat přihlášení")]
        public bool RememberMe { get; set; }
    }
}
