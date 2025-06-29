﻿using Application.CoachWeb.Comparison;
using Application.CoachWeb.Services;
using Application.CoachWeb.Strategies;
using Contracts.CoachWeb.Interfaces.Repositories;
using Contracts.CoachWeb.Interfaces.Services;
using Infrastructure.CoachWeb.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IDriverComparisonService, DriverComparisonService>();

builder.Services.AddScoped<IDriverComparisonCoordinator, DriverComparisonStrategy_AverageLapTime>();
builder.Services.AddScoped<IDriverComparisonCoordinator, DriverComparisonStrategy_TrackCondition>();
builder.Services.AddScoped<IDriverComparisonCoordinator, DriverComparisonStrategy_FastestLapTime>();


builder.Services.AddAuthentication("CoachAuth")
    .AddCookie("CoachAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "CoachAuthCookie";
        options.ExpireTimeSpan = TimeSpan.FromHours(3);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();