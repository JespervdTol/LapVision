using Contracts.App.DTO.GPS;
using Contracts.App.DTO.LapTime;
using Model.Entities;

namespace API.Helpers.Mappers
{
    public static class GPSPointMapper
    {
        public static GPSPointDTO ToDTO(this GPSPoint point)
        {
            return new GPSPointDTO
            {
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                Timestamp = point.Timestamp,
                MiniSectorNumber = point.MiniSectorNumber,
                DeltaToBest = point.DeltaToBest
            };
        }
    }
}