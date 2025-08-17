using Lunch.Domain.Enums;
using LunchWeb.Helpers;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.DiscountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LunchWeb.Controllers
{
    [Authorize(Roles = nameof(Role.MasterKucharka) + "," + nameof(Role.Admin))]
    public class DiscountController : Controller
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var discounts = await _discountService.GetAllAsync();
            return View(discounts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(List<DiscountViewModel> model)
        {
            if (!ModelState.IsValid)
            {
                var current = await _discountService.GetAllAsync();
                foreach (var row in current)
                {
                    var posted = model.FirstOrDefault(x => x.RoleId == row.RoleId);
                    if (posted != null) row.Discount = posted.Discount;
                }

                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                TempData["ErrorMessage"] = string.Join(", ", errors);
                return View(nameof(Index), current);
            }

            var result = await _discountService.UpdateAsync(model);

            if (result == UpdateResult.Updated)
                TempData["SuccessMessage"] = "Slevy byly úspěšně uloženy.";
            else
                TempData["SuccessMessage"] = "Neproběhly žádné změny (hodnoty zůstaly stejné).";

            return RedirectToAction(nameof(Index));
        }
    }
}
