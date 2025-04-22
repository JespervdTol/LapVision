using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class MiniSector
    {
        [Key]
        public int MiniSectorID { get; set; }

        [Required]
        public int LapTimeID { get; set; }

        [ForeignKey("LapTimeID")]
        public virtual LapTime LapTime { get; set; } = null!;

        public int SectorNumber { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public TimeSpan Duration => EndTime - StartTime;

        public bool? IsFasterThanBest { get; set; }
        public bool? IsFasterThanPrevious { get; set; }
    }
}