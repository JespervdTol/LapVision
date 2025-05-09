namespace Contracts.App.DTO.Circuit
{
    public class CircuitDTO
    {
        public int CircuitID { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public double StartLineLat { get; set; }
        public double StartLineLng { get; set; }

        public double RadiusMeters { get; set; } = 10;
    }
}