using Lunch.Domain.Models;
using Lunch.Shared.Policies;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.PaidFoodViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LunchWeb.Controllers
{
    [Authorize]
    public class PaidFoodController : Controller
    {
        private readonly IPaidFoodService _paidFoodService;
        private readonly UserManager<Customer> _userManager;

        public PaidFoodController(IPaidFoodService paidFoodService, UserManager<Customer> userManager)
        {
            _paidFoodService = paidFoodService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateOnly? startOfWeek)
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

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var vm = await _paidFoodService.GetWeeklyPaidFoodAsync(user.Id, requested);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(PaidFoodWeeklyViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            vm.StartOfWeek = WeekEditPolicy.NormalizeToMonday(vm.StartOfWeek);

            if (!ModelState.IsValid)
            {
                var reloadedVm = await _paidFoodService.GetWeeklyPaidFoodAsync(user.Id, vm.StartOfWeek);
                return View("Index", reloadedVm);
            }

            try
            {
                await _paidFoodService.SaveWeeklyPaidFoodAsync(user.Id, vm);
                TempData["SuccessMessage"] = "Výběr jídel byl uložen.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index), new { startOfWeek = vm.StartOfWeek.ToString("yyyy-MM-dd") });
            }

            return RedirectToAction(nameof(Index), new { startOfWeek = vm.StartOfWeek.ToString("yyyy-MM-dd") });
        }

        [HttpGet]
        public async Task<IActionResult> MealDetail(int dailyOfferId, DateOnly selectedWeek)
        {
            if (dailyOfferId <= 0)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest("Neplatné ID jídelní nabídky.");

                TempData["ErrorMessage"] = "Neplatné ID jídelní nabídky.";
                return RedirectToAction(nameof(Index), new { startOfWeek = selectedWeek.ToString("yyyy-MM-dd") });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Unauthorized("Nejste přihlášen.");

                return Unauthorized();
            }

            try
            {
                var mealOption = await _paidFoodService.GetMealOptionAsync(dailyOfferId, user.Id);
                if (mealOption == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return NotFound("Jídlo nebylo nalezeno.");

                    TempData["ErrorMessage"] = "Jídlo nebylo nalezeno.";
                    return RedirectToAction(nameof(Index), new { startOfWeek = selectedWeek.ToString("yyyy-MM-dd") });
                }

                return PartialView("~/Views/PaidFood/_MealDetailPartial.cshtml", mealOption);
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return StatusCode(500, $"Chyba při načítání detailu jídla: {ex.Message}");

                TempData["ErrorMessage"] = $"Chyba při načítání detailu jídla: {ex.Message}";
                return RedirectToAction(nameof(Index), new { startOfWeek = selectedWeek.ToString("yyyy-MM-dd") });
            }
        }
    }
}
