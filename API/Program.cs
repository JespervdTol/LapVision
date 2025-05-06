using Infrastructure.Persistence;
using Model.Entities;
using API.Services;
using API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System.Diagnostics;

var sw = Stopwatch.StartNew();

var builder = WebApplication.CreateBuilder(args);
sw.Stop();
System.Diagnostics.Debug.WriteLine($"[Startup] Builder created in {sw.ElapsedMilliseconds} ms");

sw.Restart();
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var certPath = Path.Combine(AppContext.BaseDirectory, "cert.pfx");
var certPassword = builder.Configuration["Kestrel:Certificates:Default:Password"];

builder.WebHost.ConfigureKestrel(options =>
{
    var httpPort = builder.Configuration.GetValue("ASPNETCORE_HTTP_PORT", 5082);
    var httpsPort = builder.Configuration.GetValue("ASPNETCORE_HTTPS_PORT", 7234);

    options.ListenAnyIP(httpPort);

    if (File.Exists(certPath) && !string.IsNullOrEmpty(certPassword))
    {
        options.ListenAnyIP(httpsPort, listen => listen.UseHttps(certPath, certPassword));
    }
});
sw.Stop();
System.Diagnostics.Debug.WriteLine($"[Startup] Config loaded and Kestrel configured in {sw.ElapsedMilliseconds} ms");

sw.Restart();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<LapTimeService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LapVision API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT using Bearer. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
sw.Stop();
System.Diagnostics.Debug.WriteLine($"[Startup] Services registered in {sw.ElapsedMilliseconds} ms");

sw.Restart();
var app = builder.Build();
sw.Stop();
System.Diagnostics.Debug.WriteLine($"[Startup] App built in {sw.ElapsedMilliseconds} ms");

sw.Restart();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

    if (!context.Database.CanConnect())
    {
        System.Diagnostics.Debug.WriteLine("[Startup] 🛠️ Database not found — running migrations and seeding...");

        if (app.Environment.IsDevelopment())
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                var migrateSw = Stopwatch.StartNew();
                context.Database.Migrate();
                migrateSw.Stop();
                System.Diagnostics.Debug.WriteLine($"[Startup] Database migrated in {migrateSw.ElapsedMilliseconds} ms");
            }
        }

        var seedSw = Stopwatch.StartNew();
        if (!context.Accounts.Any(a => a.Role == Model.Enums.UserRole.Admin))
        {
            DataSeeder.SeedAdmin(context, builder.Configuration);
        }
        seedSw.Stop();
        System.Diagnostics.Debug.WriteLine($"[Startup] Admin seeding (if needed) took {seedSw.ElapsedMilliseconds} ms");
    }
    else
    {
        System.Diagnostics.Debug.WriteLine("[Startup] ✅ Database already exists. Skipping migration and seeding.");
    }
}
sw.Stop();
System.Diagnostics.Debug.WriteLine($"[Startup] DB setup complete in {sw.ElapsedMilliseconds} ms");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

System.Diagnostics.Debug.WriteLine("[Startup] API ready. Listening for requests...");

app.Run();