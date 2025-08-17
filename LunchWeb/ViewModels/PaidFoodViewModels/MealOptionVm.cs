namespace LunchWeb.ViewModels.PaidFoodViewModels
{
    public class MealOptionVm
    {
        public int DailyOfferId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public decimal FinalPrice { get; set; }
        public decimal FinalPriceAfterDiscount { get; set; }
        public string? ImageUrl { get; set; }
    }
}
