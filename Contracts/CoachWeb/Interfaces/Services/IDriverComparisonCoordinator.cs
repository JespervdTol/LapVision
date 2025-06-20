using Contracts.CoachWeb.DTO;

namespace Contracts.CoachWeb.Interfaces.Services
{
    public interface IDriverComparisonCoordinator
    {
        string Name { get; }

        ComparisonResultDTO Compare(DriverReportDTO driver1, string driver1Name, DriverReportDTO driver2, string driver2Name);
    }
}