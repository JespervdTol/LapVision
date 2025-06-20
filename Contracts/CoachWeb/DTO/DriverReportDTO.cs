using Contracts.CoachWeb.DTO;

namespace Contracts.CoachWeb.DTO
{
    public class DriverReportDTO
    {
        public string DriverName { get; set; }
        public int SessionId { get; set; }
        public DateTime SessionDate { get; set; }
        public string CircuitName { get; set; }

        public List<HeatDTO> Heats { get; set; } = new();
    }
}