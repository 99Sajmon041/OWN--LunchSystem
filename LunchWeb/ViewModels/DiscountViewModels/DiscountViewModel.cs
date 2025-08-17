using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.DiscountViewModels
{
    public class DiscountViewModel
    {
        public string RoleId { get; set; } = default!;
        public string RoleName { get; set; } = default!;

        [Required(ErrorMessage = "Slevu je nutné nastavit!")]
        [Range(0, 100, ErrorMessage = "Sleva musí být 0–100 %")]
        public decimal Discount { get; set; }
    }
}