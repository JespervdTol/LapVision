using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;

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

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("App.appsettings.Development.json");
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream!)
                .Build();

            string baseUrl;
            var baseUrls = config.GetSection("ApiSettings:BaseUrls");

            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                baseUrl = DeviceInfo.DeviceType == DeviceType.Virtual
                    ? baseUrls["Emulator"]
                    : baseUrls["PhysicalAndroid"];
            }
            else
            {
                baseUrl = baseUrls["Windows"];
            }

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
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