using System.ComponentModel.DataAnnotations.Schema;

namespace Lunch.Domain.Models
{
    public class DailyOffer
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Meal))]
        public int MealId { get; set; }
        public Meal Meal { get; set; } = null!;
        public DateOnly Date { get; set; }
        public int Position { get; set; }
    }
}
