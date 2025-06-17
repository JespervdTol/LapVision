using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;

namespace Application.CoachWeb.Services
{
    public class DriverComparisonService
    {
        private readonly IEnumerable<IDriverComparisonService> _comparisonServices;

        public DriverComparisonService(IEnumerable<IDriverComparisonService> comparisonServices)
        {
            _comparisonServices = comparisonServices;
        }

        public List<ComparisonResultViewModel> CompareDrivers(
            DriverReportViewModel driver1, string driver1Name,
            DriverReportViewModel driver2, string driver2Name,
            IEnumerable<string> selectedMetricNames)
        {
            return _comparisonServices
                .Where(s => selectedMetricNames.Contains(s.Name))
                .Select(s => s.Compare(driver1, driver1Name, driver2, driver2Name))
                .ToList();
        }

        public List<string> GetAvailableComparisonMetrics()
        {
            return _comparisonServices.Select(s => s.Name).ToList();
        }
    }
}