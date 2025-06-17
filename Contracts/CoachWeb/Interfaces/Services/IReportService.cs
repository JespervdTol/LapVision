using Contracts.CoachWeb.ViewModels;
using Contracts.CoachWeb.ViewModels.Report;

namespace Contracts.CoachWeb.Interfaces.Services
{
    public interface IReportService
    {
        Task<List<DriverReportViewModel>> GetDriverReportAsync(int accountId);
        Task<DriverReportViewModel?> GetSessionReportAsync(int sessionId);
        Task<List<SessionDropdownViewModel>> GetSessionDropdownAsync(int driverId);
    }
}