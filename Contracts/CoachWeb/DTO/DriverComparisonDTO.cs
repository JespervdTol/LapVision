using Contracts.CoachWeb.DTO;
using System.Collections.Generic;

namespace Contracts.CoachWeb.DTO
{
    public class DriverComparisonDTO
    {
        public string Driver1Name { get; set; }
        public string Driver2Name { get; set; }

        public List<ComparisonResultDTO> ComparisonResults { get; set; } = new();
    }
}