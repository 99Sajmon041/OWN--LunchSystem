using Lunch.Domain.Enums;
using Lunch.Domain.Models;
using Microsoft.AspNetCore.Identity;


namespace Lunch.Infrastructure.Database
{
    public static partial class DbSeeder
    {
        public static async Task SeedAdminAndRolesAsync(UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Role.Admin.ToString()))
            {
                await roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString()));
            }

            var existingUser = await userManager.FindByEmailAsync("admin@admin.cz");
            if (existingUser == null)
            {
                var admin = new Customer
                {
                    UserName = "admin@admin.cz",
                    Email = "admin@admin.cz",
                    FirstName = "admin",
                    LastName = "Účet",
                    EmailConfirmed = true,
                    Active = true,
                    Role = Role.Admin,
                    MustChangePassword = true,
                    CardId = "xxxxxxxxxxx",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30))
                };

                var result = await userManager.CreateAsync(admin, "12345678");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Role.Admin.ToString());
                }
                else
                {
                    throw new Exception("Nepodařilo se vytvořit uživatele: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            var roles = Enum.GetNames(typeof(Role));
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
