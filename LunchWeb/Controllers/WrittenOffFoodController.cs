using Lunch.Domain.Enums;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.WrittenOffFoodViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = nameof(Role.MasterKucharka) + "," + nameof(Role.Admin))]
public class WrittenOffFoodController : Controller
{
    private readonly IWrittenOffFoodService _writtenOffFoodService;
    private const int PageSize = 40;

    public WrittenOffFoodController(IWrittenOffFoodService writtenOffFoodService)
    {
        _writtenOffFoodService = writtenOffFoodService;
    }

    public IActionResult Index(int page = 1)
    {
        var allItems = _writtenOffFoodService.GetAll().ToList();
        var totalCount = allItems.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        var items = allItems
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        var viewModel = new WrittenOffFoodListViewModel
        {
            Items = items,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(viewModel);
    }
}