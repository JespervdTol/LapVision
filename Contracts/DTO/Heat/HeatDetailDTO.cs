using Contracts.DTO.LapTime;

namespace Contracts.DTO.Heat
{
    public class HeatDetailDTO
    {
        public int HeatID { get; set; }
        public int HeatNumber { get; set; }
        public List<LapTimeDTO> LapTimes { get; set; }
    }
}