using Contracts.DTO.Heat;

namespace Contracts.DTO.Session
{
    public class SessionDetailDTO
    {
        public int SessionID { get; set; }
        public string CircuitName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<HeatOverviewDTO> Heats { get; set; }
        public TimeSpan? FastestLap { get; set; }

        public int PersonID { get; set; }  // 👈 Add this
    }
}