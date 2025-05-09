using Contracts.CoachWeb.ViewModels.Report;

namespace Infrastructure.CoachWeb.Interfaces
{
    public interface IReportRepository
    {
        Task<List<DriverReportViewModel>> GetReportByAccountIdAsync(int accountId);
    }
}