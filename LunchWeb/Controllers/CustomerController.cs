using Lunch.Domain.Enums;
using Lunch.Shared.Exceptions;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.CustomerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LunchWeb.Controllers
{
    [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.MasterKucharka))]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var appUsers = await _customerService.GetAllUsersAsync();
            return View(appUsers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var appUser = await _customerService.GetUserByIdAsync(id);
                return View(appUser);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
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
        public async Task<IActionResult> Create(CustomerCreateViewModel createModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createModel);
            }
            try
            {
                await _customerService.CreateUserAsync(createModel);
                TempData["SuccessMessage"] = $"Uživatel: {createModel.FirstName} {createModel.LastName} byl úspěšně vytvořen";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(createModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                var customer = await _customerService.GetUserByIdAsync(id);

                var updateModel = new CustomerUpdateViewModel
                {
                    Id = customer!.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    DateOfBirth = customer.DateOfBirth,
                    Role = Enum.Parse<Role>(customer.Role),
                    CardId = customer.CardId,
                    Active = customer.Active,
                    MustChangePassword = false
                };

                return View(updateModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CustomerUpdateViewModel updateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(updateModel);
            }
            try
            {
                await _customerService.UpdateUserAsync(updateModel);
                TempData["SuccessMessage"] = $"Uživatel: {updateModel.FirstName} {updateModel.LastName} upraven.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (ex is ArgumentNullException or InvalidOperationException)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(updateModel);
            }
            catch (NoChangesException ex)
            {
                TempData["InfoMessage"] = ex.Message;
                return View(updateModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value == id)
            {
                TempData["ErrorMessage"] = "Nemůžete odstranit vlastní účet.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                await _customerService.DeleteUserAsync(id);
                TempData["InfoMessage"] = "Uživatel odstraněn";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
