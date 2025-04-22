using Contracts.DTO.LapTime;
using Model.Entities;

namespace API.Helpers.Mappers
{
    public static class LapTimeMapper
    {
        public static LapTime ToModel(this CreateLapTimeRequest dto)
        {
            return new LapTime
            {
                HeatID = dto.HeatID,
                TotalTime = dto.TotalTime,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };
        }

        public static LapTimeDTO ToDTO(this LapTime lapTime)
        {
            return new LapTimeDTO
            {
                LapTimeID = lapTime.LapTimeID,
                LapNumber = lapTime.LapNumber,
                TotalTime = lapTime.TotalTime,
                StartTime = lapTime.StartTime,
                EndTime = lapTime.EndTime
            };
        }
    }
}