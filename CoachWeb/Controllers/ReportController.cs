using CoachWeb.Services.Interfaces;
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
    }
}