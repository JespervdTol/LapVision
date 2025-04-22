using Contracts.DTO.Heat;
using Contracts.DTO.LapTime;
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
                LapTimes = heat.LapTimes?
                    .Select(lt => lt.ToDTO())
                    .ToList() ?? new List<LapTimeDTO>()
            };
        }
    }
}