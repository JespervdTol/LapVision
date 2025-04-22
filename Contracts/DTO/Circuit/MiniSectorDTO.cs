using System;

namespace Contracts.DTO.Circuit
{
    public class MiniSectorDTO
    {
        public int SectorNumber { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public TimeSpan Duration => EndTime - StartTime;

        public bool? IsFasterThanBest { get; set; }
        public bool? IsFasterThanPrevious { get; set; }
    }
}