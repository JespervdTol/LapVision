using CoachWeb.Services.Interfaces;
using Contracts.CoachWeb.ViewModels.Report;
using Infrastructure.CoachWeb.Interfaces;

namespace CoachWeb.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;

        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }

        public Task<List<DriverReportViewModel>> GetDriverReportAsync(int accountId)
        {
            return _repo.GetReportByAccountIdAsync(accountId);
        }
    }
}