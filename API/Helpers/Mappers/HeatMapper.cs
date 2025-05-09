using Contracts.App.DTO.Heat;
using Contracts.App.DTO.LapTime;
using Model.Entities;

namespace API.Helpers.Mappers
{
    public static class HeatMapper
    {
        public static HeatOverviewDTO ToOverviewDTO(this Heat heat)
        {
            return new HeatOverviewDTO
            {
                HeatID = heat.HeatID,
                HeatNumber = heat.HeatNumber,
                FastestLap = heat.LapTimes?
                    .Select(lt => lt.CalculateTotalTime())
                    .Where(t => t.HasValue)
                    .Min()
            };
        }

        public static HeatDetailDTO ToDetailDTO(this Heat heat)
        {
            return new HeatDetailDTO
            {
                HeatID = heat.HeatID,
                HeatNumber = heat.HeatNumber,
                CircuitID = heat.Session.CircuitID,
                LapTimes = heat.LapTimes.Select(l => l.ToDTO()).ToList()
            };
        }
    }
}