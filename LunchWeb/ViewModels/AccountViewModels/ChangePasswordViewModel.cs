using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.AccountViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Zadejte původní heslo!")]
        [DataType(DataType.Password)]
        [Display(Name = "Původní heslo")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "Zadejte nové heslo!")]
        [DataType(DataType.Password)]
        [Display(Name = "Nové heslo")]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Hesla se neshodují!")]
        [Compare(nameof(NewPassword), ErrorMessage = "Hesla se neshodují.")]
        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
