using Contracts.DTO.Circuit;
using Contracts.DTO.GPS;

namespace Contracts.DTO.LapTime
{
    public class LapMapDTO
    {
        public int LapTimeID { get; set; }
        public List<GPSPointDTO> GPSPoints { get; set; } = new();
        public List<MiniSectorDTO> MiniSectors { get; set; } = new();
    }
}