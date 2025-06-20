using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels;
using CoachWeb.Mappers;
using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoachWeb.Controllers
{
    [Authorize(Roles = "Coach")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IDriverComparisonService _comparisonService;
        private readonly IEnumerable<IDriverComparisonCoordinator> _allStrategies;

        public ReportController(
            IReportService reportService,
            IDriverComparisonService comparisonService,
            IEnumerable<IDriverComparisonCoordinator> allStrategies)
        {
            _reportService = reportService;
            _comparisonService = comparisonService;
            _allStrategies = allStrategies;
        }

        [HttpGet]
        public async Task<IActionResult> Driver(int? personId, int? sessionId)
        {
            var drivers = await _reportService.GetAllDriversAsync();
            ViewBag.Drivers = drivers.Select(d => new DriverDropdownViewModel
            {
                PersonID = d.PersonId,
                FullName = d.FullName
            }).ToList();

            if (personId == null)
                return View("DriverReport", new List<DriverReportViewModel>());

            var sessionDTOs = await _reportService.GetDriverReportAsync(personId.Value);
            var sessions = sessionDTOs.Select(DriverReportViewModelMapper.ToViewModel).ToList();

            ViewBag.Sessions = sessions.Select(s => new SessionDropdownViewModel
            {
                SessionID = s.SessionID,
                DisplayText = $"{s.CircuitName} ({s.SessionDate:dd MMM yyyy})"
            }).ToList();

            if (sessionId != null)
            {
                var singleDTO = await _reportService.GetSessionReportAsync(sessionId.Value);
                var singleVM = singleDTO != null ? DriverReportViewModelMapper.ToViewModel(singleDTO) : null;

                return View("DriverReport", singleVM != null ? new List<DriverReportViewModel> { singleVM } : new());
            }

            return View("DriverReport", sessions);
        }

        [HttpGet]
        public async Task<IActionResult> CompareDrivers(
    int? selectedDriver1Id,
    int? selectedSession1Id,
    int? selectedDriver2Id,
    int? selectedSession2Id,
    List<string>? selectedComparisons)
        {
            var allDrivers = await _reportService.GetAllDriversAsync();

            var strategyOptions = _allStrategies.Select(s => new StrategyOptionViewModel
            {
                Id = s.GetType().Name,
                DisplayName = s.Name
            }).ToList();

            var model = new CompareDriversFormViewModel
            {
                AllDrivers = allDrivers.Select(d => new DriverDropdownViewModel
                {
                    PersonID = d.PersonId,
                    FullName = d.FullName
                }).ToList(),
                StrategyOptions = strategyOptions,
                SelectedDriver1Id = selectedDriver1Id,
                SelectedDriver2Id = selectedDriver2Id,
                SelectedSession1Id = selectedSession1Id,
                SelectedSession2Id = selectedSession2Id,
                SelectedComparisonIds = selectedComparisons ?? new List<string>()
            };

            if (selectedDriver1Id != null)
            {
                var driver1Sessions = await _reportService.GetDriverReportAsync(selectedDriver1Id.Value);
                model.Driver1Sessions = driver1Sessions.Select(s => new SessionDropdownViewModel
                {
                    SessionID = s.SessionId,
                    DisplayText = $"{s.CircuitName} ({s.SessionDate:dd MMM yyyy})"
                }).ToList();
            }

            if (selectedDriver2Id != null)
            {
                var driver2Sessions = await _reportService.GetDriverReportAsync(selectedDriver2Id.Value);
                model.Driver2Sessions = driver2Sessions.Select(s => new SessionDropdownViewModel
                {
                    SessionID = s.SessionId,
                    DisplayText = $"{s.CircuitName} ({s.SessionDate:dd MMM yyyy})"
                }).ToList();
            }

            if (selectedDriver1Id != null && selectedDriver2Id != null &&
                selectedSession1Id != null && selectedSession2Id != null &&
                selectedComparisons != null && selectedComparisons.Any())
            {
                var driver1Name = model.AllDrivers.FirstOrDefault(d => d.PersonID == selectedDriver1Id)?.FullName ?? "Driver 1";
                var driver2Name = model.AllDrivers.FirstOrDefault(d => d.PersonID == selectedDriver2Id)?.FullName ?? "Driver 2";

                var selectedStrategies = _allStrategies
                    .Where(s => selectedComparisons.Contains(s.GetType().Name))
                    .ToList();

                var result = await _comparisonService.CompareDrivers(
                    driver1Name, selectedSession1Id.Value,
                    driver2Name, selectedSession2Id.Value,
                    selectedStrategies
                );

                if (result.IsFailure)
                {
                    model.ErrorMessage = result.Error;
                }
                else
                {
                    model.Result = new DriverComparisonViewModel
                    {
                        Driver1Name = result.Value!.Driver1Name,
                        Driver2Name = result.Value.Driver2Name,
                        ComparisonResults = result.Value.ComparisonResults.Select(r => new ComparisonResultViewModel
                        {
                            MetricName = r.MetricName,
                            Driver1Value = r.Driver1Value,
                            Driver2Value = r.Driver2Value,
                            Winner = r.Winner
                        }).ToList()
                    };
                }
            }

            return View("CompareDrivers", model);
        }
    }
}