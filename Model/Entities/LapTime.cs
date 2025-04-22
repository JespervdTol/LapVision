using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class LapTime
    {
        [Key]
        public int LapTimeID { get; set; }

        [Required]
        public int HeatID { get; set; }

        [ForeignKey(nameof(HeatID))]
        public Heat Heat { get; set; }

        public TimeSpan? TotalTime { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        public int LapNumber { get; set; }

        [NotMapped]
        public bool IsManual => TotalTime.HasValue;

        public TimeSpan? CalculateTotalTime()
        {
            if (TotalTime.HasValue)
                return TotalTime;

            if (StartTime.HasValue && EndTime.HasValue)
                return EndTime.Value - StartTime.Value;

            return null;
        }

        public ICollection<MiniSector> MiniSectors { get; set; } = new List<MiniSector>();
        public ICollection<GPSPoint> GPSPoints { get; set; } = new List<GPSPoint>();
    }
}