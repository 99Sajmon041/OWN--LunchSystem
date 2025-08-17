using Lunch.Domain.Models;
using Lunch.Infrastructure.Database;
using LunchWeb.Helpers;
using LunchWeb.Repositories.Implementations;
using LunchWeb.Repositories.Interfaces;
using LunchWeb.Services.Implementations;
using LunchWeb.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LunchWeb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllersWithViews();
        services.AddAutoMapper(typeof(MappingProfile));
        services.Configure<SmtpOptions>(config.GetSection("Smtp"));
        services.AddTransient<IEmailSender, EmailSender>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnectionString")));

        services.AddIdentity<Customer, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IMealService, MealService>();
        services.AddScoped<IDailyOfferService, DailyOfferService>();
        services.AddScoped<IWrittenOffFoodService, WrittenOffFoodService>();
        services.AddScoped<IPaidFoodService, PaidFoodService>();
        services.AddScoped<IPlanningService, PlanningService>();
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<IImageService, ImageService>();

        services.AddScoped<IDailyOfferRepository, DailyOfferRepository>();
        services.AddScoped<IMealRepository, MealRepository>();
        services.AddScoped<IPaidFoodRepository, PaidFoodRepository>();
        services.AddScoped<IWrittenOffFoodRepository, WrittenOffFoodRepository>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();

        services.ConfigureApplicationCookie(cookie =>
        {
            cookie.AccessDeniedPath = "/Account/AccessDenied";
        });

        return services;
    }
}
