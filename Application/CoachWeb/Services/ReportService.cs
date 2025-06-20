using Application.CoachWeb.Mappers;
using Contracts.CoachWeb.DTO;
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

        public Task<List<DriverReportDTO>> GetDriverReportAsync(int accountId)
        {
            return _repo.GetReportByAccountIdAsync(accountId);
        }

        public async Task<DriverReportDTO?> GetSessionReportAsync(int sessionId)
        {
            var list = await _repo.GetReportBySessionIdAsync(sessionId);
            return list.FirstOrDefault();
        }

        public Task<List<DriverDropdownDTO>> GetAllDriversAsync()
        {
            return _repo.GetAllDriversAsync();
        }
    }
}