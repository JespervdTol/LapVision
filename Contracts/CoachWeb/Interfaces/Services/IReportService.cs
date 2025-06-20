using Contracts.CoachWeb.DTO;

public interface IReportService
{
    Task<List<DriverReportDTO>> GetDriverReportAsync(int accountId);
    Task<DriverReportDTO?> GetSessionReportAsync(int sessionId);
    Task<List<DriverDropdownDTO>> GetAllDriversAsync();
}