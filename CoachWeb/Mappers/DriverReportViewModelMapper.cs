using Contracts.CoachWeb.DTO;
using Contracts.CoachWeb.ViewModels.Report;

namespace CoachWeb.Mappers
{
    public static class DriverReportViewModelMapper
    {
        public static DriverReportViewModel ToViewModel(DriverReportDTO dto)
        {
            return new DriverReportViewModel
            {
                SessionID = dto.SessionId,
                SessionDate = dto.SessionDate,
                CircuitName = dto.CircuitName,
                DriverName = dto.DriverName,
                Heats = dto.Heats.Select(h => new HeatReportViewModel
                {
                    HeatNumber = h.HeatNumber,
                    Laps = h.Laps.Select(l => new LapReportViewModel
                    {
                        LapNumber = l.LapNumber,
                        TotalTime = l.LapTime
                    }).ToList()
                }).ToList()
            };
        }
    }
}