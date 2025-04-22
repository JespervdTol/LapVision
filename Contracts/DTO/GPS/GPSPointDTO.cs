using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTO.GPS
{
    public class GPSPointDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }

        // Optional, filled by backend during processing
        public int? MiniSectorNumber { get; set; }
        public TimeSpan? DeltaToBest { get; set; }
    }
}
