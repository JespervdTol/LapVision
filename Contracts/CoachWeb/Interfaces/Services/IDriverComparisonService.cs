using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;

namespace Contracts.CoachWeb.Interfaces.Services
{
    public interface IDriverComparisonService
    {
        string Name { get; }

        ComparisonResultViewModel Compare(DriverReportViewModel driver1, string driver1Name, DriverReportViewModel driver2, string driver2Name);
    }
}