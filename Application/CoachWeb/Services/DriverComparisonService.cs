using Contracts.CoachWeb.DTO;
using Contracts.CoachWeb.ErrorHandeling;
using Contracts.CoachWeb.Interfaces.Services;

namespace Application.CoachWeb.Services
{
    public class DriverComparisonService : IDriverComparisonService
    {
        private readonly IReportService _reportService;

        public DriverComparisonService(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<Result<DriverComparisonDTO>> CompareDrivers(
            string driver1Name,
            int session1Id,
            string driver2Name,
            int session2Id,
            IEnumerable<IDriverComparisonCoordinator> selectedStrategies)
        {
            var session1 = await _reportService.GetSessionReportAsync(session1Id);
            var session2 = await _reportService.GetSessionReportAsync(session2Id);

            if (session1 == null || session2 == null)
            {
                return Result<DriverComparisonDTO>.Failure(
                    "One or both sessions could not be found.",
                    ErrorType.UserError
                );
            }

            try
            {
                var results = selectedStrategies
                    .Select(strategy => strategy.Compare(session1, driver1Name, session2, driver2Name))
                    .ToList();

                return Result<DriverComparisonDTO>.Success(new DriverComparisonDTO
                {
                    Driver1Name = driver1Name,
                    Driver2Name = driver2Name,
                    ComparisonResults = results
                });
            }
            catch (Exception)
            {
                return Result<DriverComparisonDTO>.Failure(
                    "An unexpected error occurred while comparing drivers.",
                    ErrorType.SystemError
                );
            }
        }
    }
}