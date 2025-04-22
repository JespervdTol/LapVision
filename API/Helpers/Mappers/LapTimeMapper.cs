using Contracts.DTO.Circuit;
using Contracts.DTO.GPS;
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
                EndTime = lapTime.EndTime,
                GPSPoints = lapTime.GPSPoints?.Select(p => new GPSPointDTO
                {
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Timestamp = p.Timestamp,
                    MiniSectorNumber = p.MiniSectorNumber,
                    DeltaToBest = p.DeltaToBest
                }).ToList() ?? new(),

                MiniSectors = lapTime.MiniSectors?.Select(s => new MiniSectorDTO
                {
                    SectorNumber = s.SectorNumber,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    IsFasterThanBest = s.IsFasterThanBest
                }).ToList() ?? new()
            };
        }
    }
}