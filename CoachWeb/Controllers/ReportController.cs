using Application.CoachWeb.Services;
using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels;
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
        private readonly DriverComparisonService _comparisonService;
        private readonly IConfiguration _config;

        public ReportController(IReportService reportService, DriverComparisonService comparisonService, IConfiguration config)
        {
            _reportService = reportService;
            _comparisonService = comparisonService;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Driver(int? personId, int? sessionId)
        {
            var drivers = await GetAllDriversAsync();
            ViewBag.Drivers = drivers;

            if (personId == null)
                return View("DriverReport", new List<DriverReportViewModel>());

            var sessions = await _reportService.GetDriverReportAsync(personId.Value);
            ViewBag.Sessions = sessions.Select(s => new SessionDropdownViewModel
            {
                SessionID = s.SessionID,
                DisplayText = $"{s.CircuitName} ({s.SessionDate:dd MMM yyyy})"
            }).ToList();

            if (sessionId != null)
            {
                var session = await _reportService.GetSessionReportAsync(sessionId.Value);
                return View("DriverReport", session != null ? new List<DriverReportViewModel> { session } : new());
            }

            return View("DriverReport", sessions);
        }

        [HttpGet]
        public async Task<IActionResult> CompareDrivers(
    int? selectedDriver1Id, int? selectedSession1Id,
    int? selectedDriver2Id, int? selectedSession2Id,
    List<string>? selectedComparisons)
        {
            var model = new CompareDriversFormViewModel
            {
                AllDrivers = await GetAllDriversAsync(),
                AvailableComparisons = _comparisonService.GetAvailableComparisonMetrics()
            };

            if (selectedDriver1Id != null)
                model.Driver1Sessions = await _reportService.GetSessionDropdownAsync(selectedDriver1Id.Value);
            if (selectedDriver2Id != null)
                model.Driver2Sessions = await _reportService.GetSessionDropdownAsync(selectedDriver2Id.Value);

            model.SelectedDriver1Id = selectedDriver1Id;
            model.SelectedDriver2Id = selectedDriver2Id;
            model.SelectedSession1Id = selectedSession1Id;
            model.SelectedSession2Id = selectedSession2Id;

            if (selectedDriver1Id != null && selectedDriver2Id != null &&
                selectedSession1Id != null && selectedSession2Id != null &&
                selectedComparisons != null && selectedComparisons.Any())
            {
                var driver1Name = model.AllDrivers.FirstOrDefault(d => d.PersonID == selectedDriver1Id)?.FullName ?? "Driver 1";
                var driver2Name = model.AllDrivers.FirstOrDefault(d => d.PersonID == selectedDriver2Id)?.FullName ?? "Driver 2";

                var result = await _comparisonService.CompareDrivers(
                    driver1Name, selectedSession1Id.Value,
                    driver2Name, selectedSession2Id.Value,
                    selectedComparisons
                );

                if (result.IsFailure)
                {
                    model.ErrorMessage = result.Error;
                }
                else
                {
                    model.Result = result.Value!;
                }
            }

            return View("CompareDrivers", model);
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
    }
}