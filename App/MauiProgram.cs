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

            // ✅ Use localhost instead of localhost for local connections
            string apiBaseUrl = DeviceInfo.Current.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:7234/"  // Android Emulator
                : "https://localhost:7234/"; // Windows/macOS

            System.Diagnostics.Debug.WriteLine($"🌐 Using API Base URL: {apiBaseUrl}");

            // ✅ Register `HttpClient` globally (ensures `BaseAddress` is set)
            builder.Services.AddSingleton(sp =>
            {
                var handler = new HttpClientHandler();
                // Disable SSL validation for local development
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    // Accept any certificate
                    return true;
                };

                var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(apiBaseUrl),
                    Timeout = TimeSpan.FromSeconds(10) // Set a reasonable timeout
                };

                System.Diagnostics.Debug.WriteLine($"✅ HttpClient BaseAddress: {httpClient.BaseAddress}");
                return httpClient;
            });

            // ✅ Register `WeatherService` using the shared `HttpClient`
            builder.Services.AddHttpClient<WeatherService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7234/");
            });

            // ✅ Check API Connection AFTER `HttpClient` is properly configured
            var serviceProvider = builder.Services.BuildServiceProvider();
            CheckApiConnection(serviceProvider);

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static async void CheckApiConnection(IServiceProvider services)
        {
            var httpClient = services.GetRequiredService<HttpClient>();

            if (httpClient.BaseAddress == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ HttpClient BaseAddress is NOT set! API calls will fail.");
                return;
            }

            int maxRetries = 5;
            int delayMs = 3000;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var response = await httpClient.GetAsync("api/weather");
                    if (response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine("✅ API is accessible!");
                        return;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ API returned non-success status: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Attempt {i + 1} - API connection failed: {ex.Message}");
                }

                await Task.Delay(delayMs);
            }

            System.Diagnostics.Debug.WriteLine("❌ API is not accessible after multiple attempts!");
        }
    }
}
