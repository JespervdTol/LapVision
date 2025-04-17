using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices; // For DeviceInfo

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

            string apiBaseUrl;

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                if (DeviceInfo.DeviceType == DeviceType.Virtual)
                {
                    apiBaseUrl = "http://10.0.2.2:5082/";
                }
                else
                {
                    apiBaseUrl = "http://192.168.2.9:5082/";
                }
            }
            else
            {
                apiBaseUrl = "https://localhost:7234/";
            }

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(apiBaseUrl),
                Timeout = TimeSpan.FromSeconds(10)
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}