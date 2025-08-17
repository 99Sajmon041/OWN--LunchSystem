using Lunch.Domain.Models;
using LunchWeb.Helpers;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace LunchWeb.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<Customer> _signInManager;
        private readonly UserManager<Customer> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            SignInManager<Customer> signInManager,
            UserManager<Customer> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResult> LoginAsync(LoginViewModel loginViewModel)
        {
            if (loginViewModel == null)
                throw new ArgumentNullException(nameof(loginViewModel));

            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (user == null)
            {
                throw new InvalidOperationException("Uživatel neexistuje.");
            }

            if (!user.Active)
            {
                throw new UnauthorizedAccessException("Uživatel není aktivní.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);

            return new LoginResult
            {
                Succeeded = result.Succeeded,
                MustChangePassword = user.MustChangePassword
            };
        }

        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ProfileViewModel> GetProfileAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException("Uživatel není přihlášen.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("Uživatel nebyl nalezen.");
            }

            return new ProfileViewModel
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email!,
                IsAdult = user.IsAdult,
                Credit = user.Credit,
                Role = user.Role
            };
        }

        public async Task ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel)
        {
            if (changePasswordViewModel == null)
                throw new ArgumentNullException(nameof(changePasswordViewModel));

            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException("Uživatel není přihlášen.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("Uživatel nebyl nalezen.");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Změna hesla selhala: {errorMessages}");
            }

            if (user.MustChangePassword)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task TopUpCreditAsync(string userId, decimal amount)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser is null)
            {
                throw new ArgumentNullException(nameof(userId), $"Uživatel s ID {userId} nenalezen!");
            }

            var newBalance = appUser.Credit + amount;
            appUser.Credit = newBalance;

            var result = await _userManager.UpdateAsync(appUser);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("E-mail je povinný!", nameof(email));

            var user = await _userManager.FindByEmailAsync(email.Trim());
            if (user == null)
                return null;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            return tokenEncoded;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string tokenEncoded, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(tokenEncoded))
                return IdentityResult.Failed(new IdentityError { Description = "Neplatný požadavek pro reset hesla." });

            var user = await _userManager.FindByEmailAsync(email.Trim());
            if (user == null)
                return IdentityResult.Success;

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(tokenEncoded));
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);
            }
            return result;
        }
    }
}
