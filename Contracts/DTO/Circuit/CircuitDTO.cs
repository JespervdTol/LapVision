using System;

namespace Contracts.DTO.Circuit
{
    public class CircuitDTO
    {
        public int CircuitID { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public double StartLineLat { get; set; }
        public double StartLineLng { get; set; }
        public double RadiusMeters { get; set; }
    }
}