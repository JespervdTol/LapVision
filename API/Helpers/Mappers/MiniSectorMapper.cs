using Contracts.App.DTO.Circuit;
using Contracts.App.DTO.LapTime;
using Model.Entities;

namespace API.Helpers.Mappers
{
    public static class MiniSectorMapper
    {
        public static MiniSectorDTO ToDTO(this MiniSector sector)
        {
            return new MiniSectorDTO
            {
                SectorNumber = sector.SectorNumber,
                StartTime = sector.StartTime,
                EndTime = sector.EndTime,
                IsFasterThanBest = sector.IsFasterThanBest,
                IsFasterThanPrevious = sector.IsFasterThanPrevious
            };
        }
    }
}