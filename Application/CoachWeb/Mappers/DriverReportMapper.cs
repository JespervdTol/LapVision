using Contracts.CoachWeb.DTO;
using Contracts.CoachWeb.ViewModels.Report;

namespace Application.CoachWeb.Mappers
{
    public static class DriverReportMapper
    {
        public static DriverReportDTO ToDTO(DriverReportViewModel vm)
        {
            return new DriverReportDTO
            {
                DriverName = vm.DriverName,
                SessionId = vm.SessionID,
                SessionDate = vm.SessionDate,
                CircuitName = vm.CircuitName,
                Heats = vm.Heats.Select(h => new HeatDTO
                {
                    HeatNumber = h.HeatNumber,
                    Laps = h.Laps
                        .Where(l => l.TotalTime.HasValue)
                        .Select(l => new LapDTO
                        {
                            LapNumber = l.LapNumber,
                            LapTime = l.TotalTime.Value
                        }).ToList()
                }).ToList()
            };
        }
    }
}