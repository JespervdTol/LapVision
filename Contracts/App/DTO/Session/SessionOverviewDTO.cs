namespace Contracts.App.DTO.Session
{
    public class SessionOverviewDTO
    {
        public int SessionID { get; set; }
        public string CircuitName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int HeatCount { get; set; }

        public int PersonID { get; set; }  // 👈 Add this
    }
}