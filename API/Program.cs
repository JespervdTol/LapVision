using TInfrastructure.Context;
using Core.Model;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    var config = context.Configuration;
    var httpPort = config.GetValue("ASPNETCORE_HTTP_PORT", 5082);
    var httpsPort = config.GetValue("ASPNETCORE_HTTPS_PORT", 7234);
    var certPath = Path.Combine(AppContext.BaseDirectory, "cert.pfx");
    var certPassword = "jtol123@";

    System.Diagnostics.Debug.WriteLine($"🌐 Binding ports: HTTP={httpPort}, HTTPS={httpsPort}");
    options.ListenAnyIP(httpPort);

    if (File.Exists(certPath))
    {
        System.Diagnostics.Debug.WriteLine("✅ Certificate found. Enabling HTTPS.");
        options.ListenAnyIP(httpsPort, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
    }
    else
    {
        System.Diagnostics.Debug.WriteLine("⚠️ Certificate not found. HTTPS will be unavailable.");
    }
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
System.Diagnostics.Debug.WriteLine($"🧪 DB connection string: {connectionString}");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("❌ Missing DB connection string in appsettings.json.");
}

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpClient<WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7234/");
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtConfig = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

    try
    {
        dbContext.Database.Migrate();
        System.Diagnostics.Debug.WriteLine(dbContext.Database.CanConnect()
            ? "✅ DB connection successful."
            : "❌ DB connection failed!");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"❌ DB connection error: {ex.Message}");
        throw;
    }

    if (!dbContext.Accounts.Any())
    {
        System.Diagnostics.Debug.WriteLine("🛠 No accounts found. Seeding default Admin user...");

        string adminEmail = "admin@lapvision.com";
        string adminUsername = "admin";
        string adminPassword = "Admin123!";

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

        var adminAccount = new Account
        {
            Email = adminEmail,
            Username = adminUsername,
            PasswordHash = passwordHash,
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow,
            Person = new Person
            {
                FirstName = "System",
                LastName = "Admin",
                Prefix = "",
                DateOfBirth = new DateOnly(2003, 9, 30)
            }
        };

        dbContext.Accounts.Add(adminAccount);
        dbContext.SaveChanges();

        System.Diagnostics.Debug.WriteLine($"✅ Default admin created: {adminEmail} / {adminPassword}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    if (context.Request.IsHttps || !context.Request.Host.Port.HasValue || context.Request.Host.Port != 5082)
    {
        await next();
    }
    else
    {
        context.Request.Scheme = "http";
        await next();
    }
});

app.UseCors("AllowAll");
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();