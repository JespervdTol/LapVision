using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels.Report;
using Contracts.CoachWeb.Interfaces.Repositories;

namespace Application.CoachWeb.Services
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