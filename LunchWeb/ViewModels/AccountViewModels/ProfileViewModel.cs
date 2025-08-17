using Lunch.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.AccountViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Celé jméno")]
        public string FullName { get; set; } = null!;


        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Dospělý")]
        public bool IsAdult { get; set; }

        [Display(Name = "Kredit")]
        public decimal Credit { get; set; }

        public Role Role { get; set; }
    }
}
