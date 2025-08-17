using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace LunchWeb.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");

        return app;
    }

    public static IApplicationBuilder UseRequestLocalizationCs(this IApplicationBuilder app)
    {
        var culture = new CultureInfo("cs-CZ");
        var opts = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(culture),
            SupportedCultures = new[] { culture },
            SupportedUICultures = new[] { culture }
        };
        app.UseRequestLocalization(opts);
        return app;
    }
}
