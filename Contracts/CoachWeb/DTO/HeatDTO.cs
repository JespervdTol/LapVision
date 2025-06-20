namespace Contracts.CoachWeb.DTO
{
    public class HeatDTO
    {
        public int HeatNumber { get; set; }
        public List<LapDTO> Laps { get; set; } = new();
    }
}