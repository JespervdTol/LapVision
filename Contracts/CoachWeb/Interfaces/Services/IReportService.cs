using Contracts.CoachWeb.ViewModels.Report;

namespace Contracts.CoachWeb.Interfaces.Services
{
    public interface IReportService
    {
        Task<List<DriverReportViewModel>> GetDriverReportAsync(int accountId);
    }
}