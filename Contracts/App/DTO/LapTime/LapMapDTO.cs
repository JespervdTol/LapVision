using Contracts.App.DTO.Circuit;
using Contracts.App.DTO.GPS;

namespace Contracts.App.DTO.LapTime
{
    public class LapMapDTO
    {
        public int LapTimeID { get; set; }
        public List<GPSPointDTO> GPSPoints { get; set; } = new();
        public List<MiniSectorDTO> MiniSectors { get; set; } = new();
    }
}