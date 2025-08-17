using Lunch.Domain.Models;
using Lunch.Shared.Exceptions;
using LunchWeb.Helpers;
using LunchWeb.Services.Interfaces;
using LunchWeb.ViewModels.CustomerViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LunchWeb.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly UserManager<Customer> _userManager;

        public CustomerService(UserManager<Customer> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAllUsersAsync()
        {
            return await _userManager.Users
                .Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email!,
                    DateOfBirth = c.DateOfBirth,
                    Active = c.Active,
                    Role = c.Role.ToString(),
                    IsAdult = c.IsAdult,
                    Credit = c.Credit,
                    CardId = c.CardId
                })
                .ToListAsync();
        }

        public async Task<CustomerViewModel?> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("ID uživatele nesmí být prázdné.", nameof(id));
            }

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                throw new InvalidOperationException($"Uživatel s ID '{id}' nebyl nalezen.");
            }

            return new CustomerViewModel
            {
                Id = appUser.Id,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email!,
                DateOfBirth = appUser.DateOfBirth,
                Active = appUser.Active,
                Role = appUser.Role.ToString(),
                IsAdult = appUser.IsAdult,
                Credit = appUser.Credit,
                CardId = appUser.CardId
            };
        }

        public async Task CreateUserAsync(CustomerCreateViewModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Vstupní model uživatele nesmí být null.");
            }

            var customer = new Customer
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.Email,
                Role = user.Role,
                DateOfBirth = user.DateOfBirth,
                Active = user.Active,
                MustChangePassword = true,
                CardId = user.CardId
            };

            var result = await _userManager.CreateAsync(customer, user.Password);
            if (!result.Succeeded)
            {
                throw new InvalidDataException($"Vytvoření uživatele selhalo: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            var roleResult = await _userManager.AddToRoleAsync(customer, user.Role.ToString());
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"Přiřazení role selhalo: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }

        public async Task UpdateUserAsync(CustomerUpdateViewModel user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.FirstName = user.FirstName!.Trim();
            user.LastName = user.LastName!.Trim();
            user.Email = user.Email!.Trim();
            user.CardId = user.CardId!.Trim();

            var customer = await _userManager.FindByIdAsync(user.Id)
                          ?? throw new InvalidOperationException($"Uživatel s ID '{user.Id}' nebyl nalezen.");

            if (EntityComparer.IsCustomerSame(customer, user))
                throw new NoChangesException("Žádné změny nebyly provedeny.");

            var existingEmail = await _userManager.FindByEmailAsync(user.Email);
            if (existingEmail != null && existingEmail.Id != user.Id)
                throw new InvalidOperationException($"Email '{user.Email}' již používá jiný uživatel.");

            var existingName = await _userManager.FindByNameAsync(user.Email);
            if (existingName != null && existingName.Id != user.Id)
                throw new InvalidOperationException($"Uživatelské jméno '{user.Email}' již používá jiný uživatel.");

            customer.FirstName = user.FirstName;
            customer.LastName = user.LastName;
            customer.Email = user.Email;
            customer.UserName = user.Email;
            customer.Role = user.Role;
            customer.DateOfBirth = user.DateOfBirth;
            customer.Active = user.Active;
            customer.CardId = user.CardId;

            var emailChanged = !string.Equals(customer.Email, user.Email, StringComparison.OrdinalIgnoreCase);
            var roleChanged = customer.Role != user.Role;
            customer.MustChangePassword = emailChanged || roleChanged || user.MustChangePassword;

            var result = await _userManager.UpdateAsync(customer);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Aktualizace uživatele selhala: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }


        public async Task DeleteUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("ID uživatele nesmí být prázdné.", nameof(id));
            }

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                throw new InvalidOperationException($"Uživatel s ID '{id}' nebyl nalezen.");
            }

            var result = await _userManager.DeleteAsync(appUser);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Mazání uživatele selhalo: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
