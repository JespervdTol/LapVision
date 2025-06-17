using Contracts.CoachWeb.ViewModels.Report;

namespace Contracts.CoachWeb.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<List<DriverReportViewModel>> GetReportByAccountIdAsync(int accountId);
        Task<List<DriverReportViewModel>> GetReportBySessionIdAsync(int sessionId);
    }
}