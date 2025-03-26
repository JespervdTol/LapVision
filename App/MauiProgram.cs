using Core.Services;
using Microsoft.Extensions.Logging;

namespace App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            string apiBaseUrl = DeviceInfo.Current.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5082/"
                : "https://localhost:7234/";

            System.Diagnostics.Debug.WriteLine($"\uD83C\uDF10 Using API Base URL: {apiBaseUrl}");

            builder.Services.AddHttpClient<WeatherService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            Task.Run(() => CheckApiConnection(app.Services));

            return app;
        }

        private static async void CheckApiConnection(IServiceProvider services)
        {
            var weatherService = services.GetRequiredService<WeatherService>();

            await Task.Delay(5000);

            int maxRetries = 5;
            int delayMs = 3000;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var forecasts = await weatherService.GetWeatherAsync();
                    if (forecasts.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("✅ API is accessible!");
                        return;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("❌ API returned 0 items.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ API connection failed: {ex.Message}");
                }

                await Task.Delay(delayMs);
            }

            System.Diagnostics.Debug.WriteLine("❌ API is not accessible after multiple attempts!");
        }
    }
}