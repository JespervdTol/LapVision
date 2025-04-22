namespace Contracts.DTO.Heat
{
    public class HeatOverviewDTO
    {
        public int HeatID { get; set; }
        public int HeatNumber { get; set; }
        public TimeSpan? FastestLap { get; set; }
    }
}