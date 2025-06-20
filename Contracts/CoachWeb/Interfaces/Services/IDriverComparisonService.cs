using Contracts.CoachWeb.DTO;
using Contracts.CoachWeb.ErrorHandeling;

namespace Contracts.CoachWeb.Interfaces.Services
{
    public interface IDriverComparisonService
    {
        Task<Result<DriverComparisonDTO>> CompareDrivers(string driver1Name, int session1Id, string driver2Name, int session2Id, IEnumerable<IDriverComparisonCoordinator> selectedStrategies);
    }
}