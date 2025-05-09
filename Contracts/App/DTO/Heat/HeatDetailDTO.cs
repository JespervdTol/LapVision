using Contracts.App.DTO.LapTime;

namespace Contracts.App.DTO.Heat
{
    public class HeatDetailDTO
    {
        public int HeatID { get; set; }
        public int HeatNumber { get; set; }
        public int CircuitID { get; set; }
        public List<LapTimeDTO> LapTimes { get; set; }
    }
}