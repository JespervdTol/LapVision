using Contracts.DTO.GPS;
using Contracts.DTO.Circuit;

namespace Contracts.DTO.LapTime
{
    public class LapTimeDTO
    {
        public int LapTimeID { get; set; }
        public int LapNumber { get; set; }
        public TimeSpan? TotalTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int HeatID { get; set; }

        public List<MiniSectorDTO> MiniSectors { get; set; } = new();
        public List<GPSPointDTO> GPSPoints { get; set; } = new();
    }
}