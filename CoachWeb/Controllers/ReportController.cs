using Application.CoachWeb.Services;
using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace CoachWeb.Controllers
{
    [Authorize(Roles = "Coach")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IConfiguration _config;

        public ReportController(IReportService reportService, IConfiguration config)
        {
            _reportService = reportService;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Driver(int? personId)
        {
            var drivers = await GetAllDriversAsync();
            ViewBag.Drivers = drivers;

            if (personId == null)
            {
                return View("DriverReport", new List<DriverReportViewModel>());
            }

            var report = await _reportService.GetDriverReportAsync(personId.Value);
            return View("DriverReport", report);
        }

        private async Task<List<DriverDropdownViewModel>> GetAllDriversAsync()
        {
            var list = new List<DriverDropdownViewModel>();

            using var conn = new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT PersonID, firstName, prefix, lastName
                FROM Person
                WHERE personType = 'Driver'
                ORDER BY lastName, firstName", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var fullName = string.Join(" ",
                    reader["firstName"] as string,
                    reader["prefix"] as string ?? "",
                    reader["lastName"] as string);

                list.Add(new DriverDropdownViewModel
                {
                    PersonID = reader.GetInt32("PersonID"),
                    FullName = fullName.Trim()
                });
            }

            return list;
        }

        [HttpGet]
        public async Task<IActionResult> CompareDrivers(int? driver1Id, int? driver2Id, List<string>? selectedComparisons)
        {
            var drivers = await GetAllDriversAsync();
            ViewBag.Drivers = drivers;

            // Get comparison strategy names for checkboxes
            var comparisonService = HttpContext.RequestServices.GetService<DriverComparisonService>();
            ViewBag.ComparisonTypes = comparisonService.GetAvailableComparisonMetrics();

            if (driver1Id == null || driver2Id == null || selectedComparisons == null)
            {
                return View("CompareDrivers", new DriverComparisonViewModel());
            }

            var driver1Data = await _reportService.GetDriverReportAsync(driver1Id.Value);
            var driver2Data = await _reportService.GetDriverReportAsync(driver2Id.Value);

            var driver1Name = drivers.First(d => d.PersonID == driver1Id.Value).FullName;
            var driver2Name = drivers.First(d => d.PersonID == driver2Id.Value).FullName;

            var results = comparisonService.CompareDrivers(
                driver1Data.First(), driver1Name,
                driver2Data.First(), driver2Name,
                selectedComparisons
            );

            var viewModel = new DriverComparisonViewModel
            {
                Driver1Name = driver1Name,
                Driver2Name = driver2Name,
                ComparisonResults = results
            };

            return View("CompareDrivers", viewModel);
        }
    }
}