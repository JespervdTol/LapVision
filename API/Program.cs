using Core.Context;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

if (builder.Configuration is IConfigurationRoot configRoot)
{
    foreach (var provider in configRoot.Providers)
    {
        Console.WriteLine($"📄 Config provider: {provider.GetType().Name}");
    }
}

builder.WebHost.UseUrls();

builder.WebHost.ConfigureKestrel((context, options) =>
{
    var config = context.Configuration;
    var httpPort = config.GetValue("ASPNETCORE_HTTP_PORT", 5082);
    var httpsPort = config.GetValue("ASPNETCORE_HTTPS_PORT", 7234);
    var certPath = Path.Combine(AppContext.BaseDirectory, "cert.pfx");
    var certPassword = "jtol123@";

    Console.WriteLine($"🌐 Binding ports: HTTP={httpPort}, HTTPS={httpsPort}");

    options.ListenAnyIP(httpPort);

    if (File.Exists(certPath))
    {
        Console.WriteLine("✅ Cert found! Enabling HTTPS.");
        options.ListenAnyIP(httpsPort, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
    }
});

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"🧪 Connection string: {connectionString}");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("❌ Database connection string ontbreekt! Controleer je appsettings.json.");
}

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

builder.Services.AddHttpClient<WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7234/");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

    try
    {
        dbContext.Database.Migrate();

        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("✅ Database-verbinding succesvol!");
        }
        else
        {
            Console.WriteLine("❌ Database-verbinding mislukt! Controleer je instellingen.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Fout bij database-verbinding: {ex.Message}");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();