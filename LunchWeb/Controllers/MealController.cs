using AutoMapper;
using Lunch.Domain.Enums;
using Lunch.Shared.Exceptions;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.MealViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LunchWeb.Controllers
{
    [Authorize(Roles = nameof(Role.Kucharka) + "," + nameof(Role.MasterKucharka) + "," + nameof(Role.Admin))]
    public class MealController : Controller
    {
        private readonly IMealService _mealService;
        private readonly IMapper _mapper;

        public MealController(IMealService mealService, IMapper mapper)
        {
            _mealService = mealService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var meals = await _mealService.GetAllMealsAsync();
            return View(meals);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var meal = await _mealService.GetMealByIdAsync(id);
                return View(meal);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["InfoMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "Došlo k neočekávané chybě při načítání detailu jídla.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _mealService.CreateMealAsync(model);
                TempData["SuccessMessage"] = $"Jídlo „{model.FoodName}“ bylo úspěšně vytvořeno.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["ErrorMessage"] = $"Chyba validace: {ex.Message}";
                return View(model);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nepodařilo se uložit jídlo do databáze.";
                return View(model);
            }
            catch (IOException)
            {
                TempData["ErrorMessage"] = "Chyba při práci se souborem obrázku.";
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "Nedostatečná oprávnění pro uložení obrázku.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Došlo k neočekávané chybě: {ex.Message}";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Neplatné ID jídla.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var mealVm = await _mealService.GetMealByIdAsync(id);
                var formModel = _mapper.Map<MealFormViewModel>(mealVm);
                return View(formModel);
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "Došlo k neočekávané chybě při načítání jídla.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(MealFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _mealService.UpdateMealAsync(model);
                TempData["SuccessMessage"] = $"Jídlo „{model.FoodName}“ bylo úspěšně upraveno.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (NoChangesException ex)
            {
                TempData["InfoMessage"] = ex.Message;
                return View(model);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                TempData["ErrorMessage"] = $"Chyba validace: {ex.Message}";
                return View(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Záznam byl mezitím změněn. Načtěte stránku znovu.";
                return View(model);
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Nepodařilo se uložit změny do databáze.";
                return View(model);
            }
            catch (IOException)
            {
                TempData["ErrorMessage"] = "Chyba při práci se souborem obrázku.";
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "Nedostatečná oprávnění pro uložení obrázku.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Došlo k neočekávané chybě: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _mealService.DeleteMealAsync(id);
                TempData["SuccessMessage"] = $"Jídlo s ID {id} bylo úspěšně smazáno.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["InfoMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Záznam se nepodařilo odstranit (databázová chyba).";
                return RedirectToAction(nameof(Index));
            }
            catch (IOException)
            {
                TempData["InfoMessage"] = "Jídlo bylo smazáno, ale soubor obrázku se nepodařilo odstranit.";
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                TempData["InfoMessage"] = "Jídlo bylo smazáno, ale nemám oprávnění odstranit soubor obrázku.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "Došlo k neočekávané chybě při mazání jídla.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
