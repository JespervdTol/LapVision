using Contracts.CoachWeb.DTO;

namespace Contracts.CoachWeb.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<List<DriverReportDTO>> GetReportByAccountIdAsync(int accountId);
        Task<List<DriverReportDTO>> GetReportBySessionIdAsync(int sessionId);
        Task<List<DriverDropdownDTO>> GetAllDriversAsync();
    }
}