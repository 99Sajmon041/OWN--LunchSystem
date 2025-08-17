using Lunch.Domain.Models;
using Lunch.Infrastructure.Database;
using LunchWeb;
using LunchWeb.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration);

        var app = builder.Build();

        app.UseRequestLocalizationCs();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var db = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<Customer>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await db.Database.MigrateAsync();
            await DbSeeder.SeedAdminAndRolesAsync(userManager, roleManager);
            var inserted = await DbSeeder.SeedRoleDiscountsAsync(db, defaultDiscount: 0m);

            if (inserted > 0)
            {
                Console.WriteLine($"RoleDiscount seed: doplněno {inserted} záznamů.");
            }
        }

        app.UseApplicationPipeline();
        app.Run();
    }
}
