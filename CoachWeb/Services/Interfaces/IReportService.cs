using Contracts.CoachWeb.ViewModels.Report;

namespace CoachWeb.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<DriverReportViewModel>> GetDriverReportAsync(int accountId);
    }
}