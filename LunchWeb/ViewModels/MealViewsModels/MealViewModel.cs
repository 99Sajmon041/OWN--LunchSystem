using System.ComponentModel.DataAnnotations;

namespace LunchWeb.ViewModels.MealViewModels
{
    public class MealViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Název jídla")]
        public string FoodName { get; set; } = null!;

        [Display(Name = "Popis jídla")]
        public string FoodDescription { get; set; } = null!;

        [Display(Name = "Cena jídla (Kč)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Obrázek jídla")]
        public string ImageUrl { get; set; } = null!;
    }
}