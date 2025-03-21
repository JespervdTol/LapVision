using Core.Context;
using Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5082);

    var port = 7234;
    var certPath = @"C:\Users\jespe\source\repos\LapVision\API\cert.pfx";
    var certPassword = "jtol123@";

    var directoryPath = @"C:\Users\jespe\source\repos\LapVision\";

    System.Diagnostics.Debug.WriteLine($"🔍 Checking parent directory: {directoryPath}");

    if (Directory.Exists(directoryPath))
    {
        System.Diagnostics.Debug.WriteLine("✅ Parent directory exists. Listing contents...");
        var directories = Directory.GetDirectories(directoryPath);
        foreach (var dir in directories)
        {
            System.Diagnostics.Debug.WriteLine($"📁 Found directory: {dir}");
        }
    }
    else
    {
        System.Diagnostics.Debug.WriteLine("❌ Parent directory does NOT exist. Something is blocking access.");
    }

    var files = Directory.GetFiles(directoryPath);
    foreach (var file in files)
    {
        System.Diagnostics.Debug.WriteLine($"📄 Found file: {file}");
    }

    System.Diagnostics.Debug.WriteLine($"🔍 Checking certificate at: {certPath}");

    if (File.Exists(certPath))
    {
        System.Diagnostics.Debug.WriteLine("✅ File.Exists() returned TRUE. Cert should be accessible.");
    }
    else
    {
        System.Diagnostics.Debug.WriteLine("❌ File.Exists() returned FALSE. Cert is NOT found.");
    }

    if (File.Exists(certPath))
    {
        options.ListenAnyIP(port, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
        System.Diagnostics.Debug.WriteLine("✅ Cert found! Running with HTTPS.");
    }
    else
    {
        System.Diagnostics.Debug.WriteLine("⚠️ SSL Certificate not found. Running HTTPS without cert.");
        options.ListenAnyIP(port);
    }
});

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

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
            System.Diagnostics.Debug.WriteLine("✅ Database-verbinding succesvol!");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("❌ Database-verbinding mislukt! Controleer je instellingen.");
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"❌ Fout bij database-verbinding: {ex.Message}");
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