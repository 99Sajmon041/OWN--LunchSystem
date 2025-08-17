using Lunch.Api.Application.Kiosk;
using Lunch.Api.Infrastructure.Idempotency;
using Lunch.Domain.Models;
using Lunch.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddIdentity<Customer, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.Configure<KioskServiceOptions>(builder.Configuration.GetSection("Kiosk"));
builder.Services.AddSingleton<IIdempotencyStore, MemoryIdempotencyStore>();
builder.Services.AddScoped<IKioskService, KioskService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
