using Microsoft.EntityFrameworkCore;

namespace Lunch.Domain.Models
{
    public class Meal
    {
        public int Id { get; set; }
        public string FoodName { get; set; } = null!;
        public string FoodDescription { get; set; } = null!;

        [Precision(10, 2)]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}
