using Lunch.Shared.Policies;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.DailyOfferViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LunchWeb.Controllers
{
    [Authorize]
    public class DailyOfferController : Controller
    {
        private readonly IDailyOfferService _dailyOfferService;

        public DailyOfferController(IDailyOfferService dailyOfferService)
        {
            _dailyOfferService = dailyOfferService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateOnly? startOfWeek = null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var currentWeek = WeekEditPolicy.NormalizeToMonday(today);
            var minWeek = WeekEditPolicy.GetLaunchWeekStart();

            var rawRequested = startOfWeek ?? currentWeek;
            var requested = WeekEditPolicy.NormalizeToMonday(rawRequested);

            if (requested < minWeek)
                return RedirectToAction(nameof(Index), new { startOfWeek = currentWeek.ToString("yyyy-MM-dd") });

            if (!startOfWeek.HasValue || rawRequested != requested)
                return RedirectToAction(nameof(Index), new { startOfWeek = requested.ToString("yyyy-MM-dd") });

            try
            {
                var vm = await _dailyOfferService.GetWeeklyOfferAsync(requested);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Chyba při načítání týdenní nabídky: {ex.Message}";
                return View(new DailyOfferWeeklyViewModel { StartOfWeek = requested });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(DailyOfferWeeklyViewModel viewModel)
        {
            viewModel.StartOfWeek = WeekEditPolicy.NormalizeToMonday(viewModel.StartOfWeek);
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (!ModelState.IsValid)
            {
                var vmInvalid = await _dailyOfferService.GetWeeklyOfferAsync(viewModel.StartOfWeek);
                return View("Index", vmInvalid);
            }

            if (!WeekEditPolicy.CanEditWeek(viewModel.StartOfWeek, today))
            {
                ModelState.AddModelError(string.Empty,
                    "Tento týden nelze upravit. Zamčený je vždy bezprostředně následující týden.");
                var vmLocked = await _dailyOfferService.GetWeeklyOfferAsync(viewModel.StartOfWeek);
                return View("Index", vmLocked);
            }

            await _dailyOfferService.SaveWeeklyOfferAsync(viewModel);
            TempData["SuccessMessage"] = "Týdenní nabídka byla úspěšně uložena.";

            return RedirectToAction(nameof(Index), new
            {
                startOfWeek = viewModel.StartOfWeek.ToString("yyyy-MM-dd")
            });
        }
    }
}
