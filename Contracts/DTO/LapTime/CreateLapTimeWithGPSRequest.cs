using Contracts.DTO.Circuit;
using Contracts.DTO.GPS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTO.LapTime
{
    public class CreateLapTimeWithGPSRequest
    {
        public int HeatID { get; set; }
        public int LapNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<MiniSectorDTO> MiniSectors { get; set; } = new();
        public List<GPSPointDTO> GPSPoints { get; set; } = new();
    }
}
