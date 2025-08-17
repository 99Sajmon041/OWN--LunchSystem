using LunchWeb.Helpers;
using LunchWeb.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Identity;

namespace LunchWeb.Services.Interfaces
{
    public interface IAccountService
    {
        Task<LoginResult> LoginAsync(LoginViewModel loginViewModel);
        Task LogOutAsync();
        Task<ProfileViewModel> GetProfileAsync();
        Task ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel);
        Task TopUpCreditAsync(string userId, decimal amount);
        Task<string?> GeneratePasswordResetTokenAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string email, string tokenEncoded, string newPassword);
    }
}
