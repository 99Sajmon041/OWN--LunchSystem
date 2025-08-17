using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "E-mail je povinný.")]
        [EmailAddress(ErrorMessage = "Zadejte platný e-mail.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Heslo je povinné.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Heslo musí mít alespoň {2} znaků.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nové heslo")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Potvrzení hesla je povinné.")]
        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        [Compare(nameof(Password), ErrorMessage = "Hesla se neshodují.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        [HiddenInput(DisplayValue = false)]
        public string Token { get; set; } = null!;
    }
}
