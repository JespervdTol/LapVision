using System.ComponentModel.DataAnnotations;

namespace Contracts.DTO.LapTime
{
    public class CreateLapTimeRequest
    {
        [Required]
        public int HeatID { get; set; }

        public TimeSpan? TotalTime { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}