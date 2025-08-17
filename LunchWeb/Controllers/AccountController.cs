using Lunch.Domain.Models;
using Lunch.Shared.Policies;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LunchWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<Customer> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IPlanningService _planningService;
        public AccountController(IAccountService accountService,
            UserManager<Customer> userManager,
            IEmailSender emailSender,
            IPlanningService planningService)
        {
            _accountService = accountService;
            _userManager = userManager;
            _emailSender = emailSender;
            _planningService = planningService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }
            string msg = string.Empty;
            var today = WeekEditPolicy.GetTodayInPrague();
            if (today.DayOfWeek == DayOfWeek.Friday)
            {
                var (show, message) = await _planningService.CheckNextNextWeekAsync(today, HttpContext.RequestAborted);
                if (show)
                    msg = message;
            }
            try
            {
                var result = await _accountService.LoginAsync(loginModel);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(nameof(loginModel.Password), "Neplatný E-mail, nebo heslo!");
                    return View(loginModel);
                }
                if (result.MustChangePassword)
                {
                    TempData["InfoMessage"] = "Nastala změna v údaji, je nutné si změnit heslo!";
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        TempData["PlanningDanger"] = msg;
                    }
                    return RedirectToAction("ChangePassword");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(loginModel);
            }
            if (!string.IsNullOrWhiteSpace(msg))
            {
                TempData["PlanningDanger"] = msg;
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _accountService.LogOutAsync();
            TempData["InfoMessage"] = "Uživatel byl odhlášen";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var profileModel = await _accountService.GetProfileAsync();
                return View(profileModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel passModel)
        {
            if (!ModelState.IsValid)
            {
                return View(passModel);
            }
            if (passModel.OldPassword == passModel.NewPassword)
            {
                ModelState.AddModelError(nameof(passModel.NewPassword), "Původní a nové heslo se nesmí shodovat !");
                return View(passModel);
            }
            try
            {
                await _accountService.ChangePasswordAsync(passModel);
                TempData["InfoMessage"] = "Heslo bylo úspěšně změněno";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(passModel);
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> TopUpCredit()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser is null)
                return RedirectToAction("Index", "Home");

            var vm = new TopUpCreditViewModel
            {
                Id = appUser.Id,
                FullName = appUser.FullName,
                Amount = 100
            };
            return View(vm);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TopUpCredit(TopUpCreditViewModel creditModel)
        {
            if (!ModelState.IsValid)
            {
                return View(creditModel);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser is null)
                return RedirectToAction("Index", "Home");

            try
            {
                await _accountService.TopUpCreditAsync(currentUser.Id, creditModel.Amount);
                TempData["SuccessMessage"] = $"Částka {creditModel.Amount:N2} Kč byla úspěšně připsána.";
                return RedirectToAction(nameof(Profile));
            }
            catch (ArgumentNullException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(creditModel);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(creditModel);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = await _accountService.GeneratePasswordResetTokenAsync(model.Email);

            if (token != null)
            {
                var callbackUrl = Url.Action(nameof(ResetPassword), "Account",
                    new { email = model.Email.Trim(), token }, protocol: Request.Scheme);

                try
                {
                    await _emailSender.SendAsync(model.Email.Trim(), "Obnova hesla",
                        $"<p>Klikněte pro obnovu hesla: <a href=\"{callbackUrl}\">Obnovit heslo</a></p>");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Chyba při odesílání e-mailu: {ex.Message}";
                    return View(model);
                }
            }

            TempData["InfoMessage"] = "Pokud účet existuje, poslali jsme na e-mail instrukce k obnovení hesla.";
            return RedirectToAction(nameof(Login));
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                TempData["ErrorMessage"] = "Neplatný odkaz pro obnovu hesla.";
                return RedirectToAction(nameof(Login));
            }
            return View(new ResetPasswordViewModel { Email = email, Token = token });
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var result = await _accountService.ResetPasswordAsync(model.Email, model.Token, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(nameof(model.ConfirmPassword), error.Description);
                    }
                    return View(model);
                }
            }
            TempData["SuccessMessage"] = "Vaše heslo bylo úspěšně změněno. Přihlaste se novým heslem.";
            return RedirectToAction(nameof(Login));
        }


        [AllowAnonymous]
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Request.Headers["Referer"].ToString();

            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                returnUrl = Url.Action("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
