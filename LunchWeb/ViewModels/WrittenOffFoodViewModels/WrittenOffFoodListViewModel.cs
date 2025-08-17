namespace LunchWeb.ViewModels.WrittenOffFoodViewModels
{
    public class WrittenOffFoodListViewModel
    {
        public List<WrittenOffFoodViewModel> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
