using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;
using Contracts.CoachWeb.ErrorHandeling;

namespace Application.CoachWeb.Services
{
    public class DriverComparisonService
    {
        private readonly IEnumerable<IDriverComparisonService> _comparisonStrategies;
        private readonly IReportService _reportService;

        public DriverComparisonService(
            IEnumerable<IDriverComparisonService> comparisonStrategies,
            IReportService reportService)
        {
            _comparisonStrategies = comparisonStrategies;
            _reportService = reportService;
        }

        public List<string> GetAvailableComparisonMetrics()
        {
            return _comparisonStrategies.Select(s => s.Name).ToList();
        }

        public async Task<Result<DriverComparisonViewModel>> CompareDrivers(string driver1Name, int session1Id, string driver2Name, int session2Id, IEnumerable<string> selectedMetrics)
        {
            var session1 = await _reportService.GetSessionReportAsync(session1Id);
            var session2 = await _reportService.GetSessionReportAsync(session2Id);

            if (session1 == null || session2 == null)
            {
                return Result<DriverComparisonViewModel>.Failure(
                    "One or both sessions could not be found.",
                    ErrorType.UserError
                );
            }

            try
            {
                var results = _comparisonStrategies
                    .Where(s => selectedMetrics.Contains(s.Name))
                    .Select(s => s.Compare(session1, driver1Name, session2, driver2Name))
                    .ToList();

                return Result<DriverComparisonViewModel>.Success(new DriverComparisonViewModel
                {
                    Driver1Name = driver1Name,
                    Driver2Name = driver2Name,
                    ComparisonResults = results
                });
            }
            catch (Exception)
            {
                return Result<DriverComparisonViewModel>.Failure(
                    "An unexpected error occurred while comparing drivers.",
                    ErrorType.SystemError
                );
            }
        }
    }
}