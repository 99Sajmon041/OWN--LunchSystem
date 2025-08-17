using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.MealViewModels
{
    public class MealFormViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Název jídla")]
        [Required(ErrorMessage = "Název jídla je povinný.")]
        [StringLength(100, ErrorMessage = "Název jídla může mít maximálně 100 znaků.")]
        public string FoodName { get; set; } = null!;

        [Display(Name = "Popis jídla")]
        [Required(ErrorMessage = "Popis jídla je povinný.")]
        [StringLength(300, ErrorMessage = "Popis jídla může mít maximálně 300 znaků.")]
        public string FoodDescription { get; set; } = null!;

        [Display(Name = "Cena jídla (Kč)")]
        [Required(ErrorMessage = "Cena jídla je povinná.")]
        [Range(20, 1000.00, ErrorMessage = "Cena musí být mezi 20 a 1000 Kč.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }


        [Display(Name = "Obrázek jídla")]
        public IFormFile? ImageFile { get; set; }

        public string? ImageUrl { get; set; }
    }
}
