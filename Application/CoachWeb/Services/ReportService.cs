using Contracts.CoachWeb.Interfaces.Repositories;
using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels;
using Contracts.CoachWeb.ViewModels.Report;

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

        public async Task<List<SessionDropdownViewModel>> GetSessionDropdownAsync(int driverId)
        {
            var sessions = await _repo.GetReportByAccountIdAsync(driverId);
            return sessions.Select(s => new SessionDropdownViewModel
            {
                SessionID = s.SessionID,
                DisplayText = $"{s.CircuitName} ({s.SessionDate:dd MMM yyyy})"
            }).ToList();
        }
        public async Task<DriverReportViewModel?> GetSessionReportAsync(int sessionId)
        {
            var sessions = await _repo.GetReportBySessionIdAsync(sessionId);
            return sessions.FirstOrDefault();
        }
    }
}