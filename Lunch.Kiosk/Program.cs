using Lunch.Kiosk;
using Lunch.Kiosk.Options;
using Lunch.Kiosk.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(cfg =>
            {
                cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((ctx, services) =>
            {
                services.Configure<KioskOptions>(ctx.Configuration.GetSection("Kiosk"));

                services.AddHttpClient<ICardService, CardService>()
                        .ConfigureHttpClient((sp, http) =>
                        {
                            var opt = sp.GetRequiredService<IOptions<KioskOptions>>().Value;
                            http.BaseAddress = new Uri(opt.ApiBaseUrl);
                            http.Timeout = TimeSpan.FromSeconds(opt.TimeoutSeconds);
                        });

                services.AddTransient<Form1>();
            })
            .Build();

        Application.Run(host.Services.GetRequiredService<Form1>());
    }
}
