using System;

namespace Contracts.App.DTO.Circuit
{
    public class MiniSectorDTO
    {
        public int SectorNumber { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool? IsFasterThanBest { get; set; }
        public bool? IsFasterThanPrevious { get; set; }
    }
}