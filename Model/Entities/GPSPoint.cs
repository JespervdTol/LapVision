using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities
{
    public class GPSPoint
    {
        [Key]
        public int GPSPointID { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }

        public int? MiniSectorNumber { get; set; }

        public TimeSpan? DeltaToBest { get; set; }

        public int LapTimeID { get; set; }

        [ForeignKey("LapTimeID")]
        public virtual LapTime LapTime { get; set; } = null!;
    }
}
