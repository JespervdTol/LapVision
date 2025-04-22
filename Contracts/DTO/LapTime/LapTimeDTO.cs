namespace Contracts.DTO.LapTime
{
    public class LapTimeDTO
    {
        public int LapTimeID { get; set; }
        public int LapNumber { get; set; }
        public TimeSpan? TotalTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}